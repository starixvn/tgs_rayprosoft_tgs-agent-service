using Microsoft.Extensions.Logging;
using Tgs.Agent.Core.Models;
using Tgs.Core.Infrastructures.Security;
using Tgs.ServiceUtility;

namespace Tgs.Agent.Core.ApplicationServices;

internal class AddAgentByPlayerUpgradeQueryService(
    IAgentRepository repository,
    IPasswordHandler passwordHandler,
    IHashProvider hashProvider,
    ILogger<AddAgentByPlayerUpgradeQueryService> logger)
    : IGetService<AddAgentByPlayerUpgradeQuery, Guid>
{
    public async ValueTask<Guid> GetAsync(AddAgentByPlayerUpgradeQuery command, CancellationToken cancellationToken = default)
    {
        var (salt, pwd) = passwordHandler.GenerateCredential(hashProvider.Hash(command.PlayerCode));

        var agentId = Guid.NewGuid();

        logger.LogInformation("brand:{BrandId}, agent:{Agent}, currencyCode:{CurrencyCode}",
            command.BrandId,
            agentId,
            command.CurrencyCode);

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
            Password = pwd,
            Salt = salt,
            RealName = command.RealName,
            Type = command.Type,
            Remarks = command.Remarks,
            Email = command.Email,
            Phone = command.Phone,
            NegativeAgentDividendCarryForward = command.NegativeAgentDividendCarryForward,
            EditorUsername = command.EditorUsername
        }, cancellationToken).ConfigureAwait(false);

        await repository.CreateAgentPlayerRelationAsync(command.BrandId,
            agentId,
            command.PlayerCode,
            cancellationToken);

        return agentId;
    }
}