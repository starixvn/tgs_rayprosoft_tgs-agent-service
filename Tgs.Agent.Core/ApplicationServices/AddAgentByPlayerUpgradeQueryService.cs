using System.Text.Json;
using System.Text.Json.Nodes;
using Json.Patch;
using Json.Pointer;
using Microsoft.Extensions.Logging;
using Tgs.Agent.Core.Models;
using Tgs.Agent.Core.Models.Enums;
using Tgs.BrandCertificate.Clients;
using Tgs.PlayerService.Client;
using Tgs.SerialNumberGenerator;
using Tgs.ServiceUtility;

namespace Tgs.Agent.Core.ApplicationServices;

internal class AddAgentByPlayerUpgradeQueryService(
    IAgentRepository repository,
    IPlayerServiceClient playerServiceClient,
    IBrandServiceClient brandServiceClient,
    ISerialNumberGenerator serialNumberGenerator,
    ILogger<AddAgentByPlayerUpgradeQueryService> logger)
    : IGetService<AddAgentByPlayerUpgradeQuery, Guid>
{
    public async ValueTask<Guid> GetAsync(AddAgentByPlayerUpgradeQuery command,
        CancellationToken cancellationToken = default)
    {
        var agentId = Guid.NewGuid();

        logger.LogInformation("brand:{BrandId}, agent:{Agent}, currencyCode:{CurrencyCode}, request: {command}",
            command.BrandId,
            agentId,
            command.CurrencyCode,
            JsonSerializer.Serialize(command));

        var relationAgentId = await repository
            .CheckAgentPlayerRelationExistsAsync(command.PlayerCode, cancellationToken)
            .ConfigureAwait(false);

        if (relationAgentId != null)
        {
            return relationAgentId.Value;
        }

        var agent = new AgentModel
        {
            AgentId = agentId,
            WalletId = command.PlayerWalletId,
            Username = command.Username,
            BrandId = command.BrandId,
            LevelType = command.LevelType,
            ParentAgentId = command.ParentAgentId,
            Password = command.Password,
            Salt = command.Salt,
            RealName = command.RealName ?? "",
            Type = command.Type,
            Remarks = command.Remarks,
            Email = command.Email ?? "",
            Phone = command.Phone ?? "",
            NegativeAgentDividendCarryForward = command.NegativeAgentDividendCarryForward,
            EditorUsername = command.EditorUsername
        };

        await repository.CreateAgentAsync(agent, cancellationToken).ConfigureAwait(false);

        await playerServiceClient.UpgradePlayerToAgentAsync(command.BrandId,
            agentId,
            command.PlayerCode,
            command.EditorUsername,
            cancellationToken);

        var fee = await brandServiceClient.GetBrandProductFeeInfoAsync(command.BrandId, cancellationToken);

        await repository.CreateAgentProductFeeAsync(
            agent.BrandId, 
            agentId,
            fee?.CardGamePercentage ?? 0M,
            fee?.LiveDealerPercentage ?? 0M,
            fee?.LotteryPercentage ?? 0M,
            fee?.SlotPercentage ?? 0M,
            fee?.SportsPercentage ?? 0M,
            fee?.VirtualSportsPercentage ?? 0M,
            fee?.CardGamePercentage ?? 0M,
            fee?.LiveDealerPercentage ?? 0M,
            fee?.LotteryPercentage ?? 0M,
            fee?.SlotPercentage ?? 0M,
            fee?.SportsPercentage ?? 0M,
            fee?.VirtualSportsPercentage ?? 0M,
            command.EditorUsername,
            cancellationToken);

        var patchOperations = Array.Empty<PatchOperation>();

        var parentAgent = command.ParentAgentId is not null
            ? await repository.GetAgentByIdAsync(command.ParentAgentId.Value, cancellationToken)
            : null;

        var createMainSnapshot = new
        {
            agent.Username,
            ParentAgentUsername = parentAgent?.Username,
            Type = agent.Type.ToString(),
            LevelType = agent.LevelType.ToString(),
            agent.DividendPercentage,
            AgentDividendConditions = Array.Empty<AgentDividendConditionModel>(),
            agent.Remarks,
            Status = AgentStatus.Enabled,
            CardGamePercentage = fee?.CardGamePercentage ?? 0M,
            LiveDealerPercentage = fee?.LiveDealerPercentage ?? 0M,
            LotteryPercentage = fee?.LotteryPercentage ?? 0M,
            SlotPercentage = fee?.SlotPercentage ?? 0M,
            SportsPercentage = fee?.SportsPercentage ?? 0M,
            VirtualSportsPercentage = fee?.VirtualSportsPercentage ?? 0M,
        };

        patchOperations =
        [
            .. patchOperations,
            PatchOperation.Add(
                JsonPointer.Parse(""),
                JsonNode.Parse(JsonSerializer.Serialize(
                    createMainSnapshot,
                    AgentUtility.SerializerOptions)))
        ];

        var changeSets = new JsonPatch(patchOperations);

        var eventId = await serialNumberGenerator
            .GenerateAsync(cancellationToken)
            .ConfigureAwait(false);

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