using Json.Patch;
using Json.Pointer;
using System.Text.Json.Nodes;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Tgs.Agent.Core.Models;
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

        var fee = await brandServiceClient.GetBrandProductFeeInfoAsync(command.BrandId, cancellationToken);

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
            CommissionPercentage = command.CommissionPercentage,
            DividendPercentage = command.DividendPercentage,
            CardGameRebatePercentage = fee?.CardGameRebatePercentage ?? 0,
            LiveDealerRebatePercentage = fee?.LiveDealerRebatePercentage ?? 0,
            LotteryRebatePercentage = fee?.LotteryRebatePercentage ?? 0,
            SlotRebatePercentage = fee?.SlotRebatePercentage ?? 0,
            SportsRebatePercentage = fee?.SportsRebatePercentage ?? 0,
            VirtualSportsRebatePercentage = fee?.VirtualSportsRebatePercentage ?? 0,
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

        var patchOperations = Array.Empty<PatchOperation>();

        patchOperations = [
            .. patchOperations,
            PatchOperation.Add(
                JsonPointer.Parse(""),
                JsonNode.Parse(JsonSerializer.Serialize(
                    agent,
                    AgentUtility.SerializerOptions))),
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