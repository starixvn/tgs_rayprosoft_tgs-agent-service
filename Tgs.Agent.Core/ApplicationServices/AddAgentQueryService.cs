using Json.Patch;
using Json.Pointer;
using System.Text.Json.Nodes;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Tgs.Agent.Core.Models;
using Tgs.Agent.Core.Models.Enums;
using Tgs.BrandCertificate.Clients;
using Tgs.CashService;
using Tgs.CashService.Client;
using Tgs.Core.Infrastructures.Security;
using Tgs.ServiceUtility;
using Tgs.SerialNumberGenerator;

namespace Tgs.Agent.Core.ApplicationServices;

internal class AddAgentQueryService(
    IAgentRepository repository,
    IPasswordHandler passwordHandler,
    ICashServiceClient cashServiceClient,
    IBrandServiceClient brandServiceClient,
    ISerialNumberGenerator serialNumberGenerator,
    ILogger<AddAgentQueryService> logger)
    : IGetService<AddAgentQuery, Guid>
{
    public async ValueTask<Guid> GetAsync(AddAgentQuery command, CancellationToken cancellationToken = default)
    {
        var (salt, pwd) = passwordHandler.GenerateCredential(command.Password);

        var agentId = Guid.NewGuid();

        logger.LogInformation("brand:{BrandId}, agent:{Agent}, currencyCode:{CurrencyCode}",
            command.BrandId,
            agentId,
            command.CurrencyCode);

        var fee = await brandServiceClient.GetBrandProductFeeInfoAsync(command.BrandId,cancellationToken);

        var walletId = await cashServiceClient.CreateNewCashAsync(
            command.BrandId,
            agentId.ToString(),
            command.CurrencyCode,
            CashType.Agent,
            cancellationToken).ConfigureAwait(false);

        var eventId = await serialNumberGenerator
            .GenerateAsync(cancellationToken)
            .ConfigureAwait(false);

        var agent = new AgentModel
        {
            AgentId = agentId,
            WalletId = walletId,
            Username = command.Username,
            BrandId = command.BrandId,
            LevelType = command.LevelType,
            CardGameRebatePercentage = command.CardGameRebatePercentage,
            CommissionPercentage = command.CommissionPercentage,
            DividendPercentage = command.DividendPercentage,
            LiveDealerRebatePercentage = command.LiveDealerRebatePercentage,
            LotteryRebatePercentage = command.LotteryRebatePercentage,
            SlotRebatePercentage = command.SlotRebatePercentage,
            SportsRebatePercentage = command.SportsRebatePercentage,
            VirtualSportsRebatePercentage = command.VirtualSportsRebatePercentage,
            ParentAgentId = command.ParentAgentId,
            Password = pwd,
            Salt = salt,
            RealName = command.RealName,
            Type = command.Type,
            Remarks = command.Remarks,
            Email = command.Email,
            Phone = command.Phone,
            NegativeAgentDividendCarryForward = command.NegativeAgentDividendCarryForward,
            EditorUsername = command.EditorUsername
        };

        await repository.CreateAgentAsync(agent, cancellationToken).ConfigureAwait(false);

        await repository.CreateAgentProductFeeAsync(agentId,
            fee?.CardGamePercentage ?? 0M,
            fee?.LiveDealerPercentage ?? 0M,
            fee?.LotteryPercentage ?? 0M,
            fee?.SlotPercentage ?? 0M,
            fee?.SportsPercentage ?? 0M,
            fee?.VirtualSportsPercentage ?? 0M,
            command.EditorUsername,
            cancellationToken);

        var patchOperations = Array.Empty<PatchOperation>();

        var createMainSnapshot = new
        {
            agent.BrandId,
            agent.AgentId,
            agent.Username,
            Type = agent.Type.ToString(),
            LevelType = agent.LevelType.ToString(),
            agent.DividendPercentage,
            AgentDividendConditions = Array.Empty<AgentDividendConditionModel>(),
            agent.Remarks,
            Status = AgentStatus.Enabled,
            DefaultWalletId = walletId,
            CardGamePercentage = fee?.CardGamePercentage ?? 0M,
            LiveDealerPercentage = fee?.LiveDealerPercentage ?? 0M,
            LotteryPercentage = fee?.LotteryPercentage ?? 0M,
            SlotPercentage = fee?.SlotPercentage ?? 0M,
            SportsPercentage = fee?.SportsPercentage ?? 0M,
            VirtualSportsPercentage = fee?.VirtualSportsPercentage ?? 0M,
        };

        patchOperations = [
            .. patchOperations,
            PatchOperation.Add(
                JsonPointer.Parse(""),
                JsonNode.Parse(JsonSerializer.Serialize(
                    createMainSnapshot,
                    AgentUtility.SerializerOptions)))
        ];

        var changeSets = new JsonPatch(patchOperations);

        await repository.CreateAgentEventAsync(
            eventId,
            command.BrandId,
            agentId,
            EventAction.Create,
            JsonSerializer.SerializeToDocument(changeSets, AgentUtility.SerializerOptions),
            command.EditorUsername,
            cancellationToken).ConfigureAwait(false);

        return agentId;
    }
}