using System.Text.Json;
using Microsoft.Extensions.Logging;
using Tgs.Agent.Core.Models;
using Tgs.PlayerService.Client;
using Tgs.ServiceUtility;

namespace Tgs.Agent.Core.ApplicationServices;

internal class AddAgentByPlayerUpgradeQueryService(
    IAgentRepository repository,
    IPlayerServiceClient playerServiceClient,
    ILogger<AddAgentByPlayerUpgradeQueryService> logger)
    : IGetService<AddAgentByPlayerUpgradeQuery, Guid>
{
    public async ValueTask<Guid> GetAsync(AddAgentByPlayerUpgradeQuery command, CancellationToken cancellationToken = default)
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

        await repository.CreateAgentAsync(new AgentModel
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
        }, cancellationToken).ConfigureAwait(false);

        await playerServiceClient.UpgradePlayerToAgentAsync(command.BrandId,
            agentId,
            command.PlayerCode,
            command.EditorUsername,
            cancellationToken);

        return agentId;
    }
}