using Microsoft.Extensions.Logging;
using Tgs.Agent.Core.Models;
using Tgs.Agent.Dividend.Client;
using Tgs.Agent.Dividend.Client.Models;
using Tgs.CashService;
using Tgs.CashService.Client;
using Tgs.Core.Infrastructures.Security;
using Tgs.ServiceUtility;

namespace Tgs.Agent.Core.ApplicationServices;

internal class AddAgentByPlayerUpgradeQueryService(
    IAgentRepository repository,
    IPasswordHandler passwordHandler,
    ICashServiceClient cashServiceClient,
    IAgentContractClient agentContractClient,
    IBrandContractClient brandContractClient,
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

        var walletId = await cashServiceClient.CreateNewCashAsync(
            command.BrandId,
            agentId.ToString(),
            command.CurrencyCode,
            CashType.Agent,
            cancellationToken).ConfigureAwait(false);

        var brandContractId = command.BrandContractId ??
                              (await brandContractClient.GetBrandContractAsync(command.BrandId, cancellationToken)
                                  .FirstAsync(cancellationToken))!.Id;

        await agentContractClient.CreateAgentContractAsync (
            new CreateAgentContractRequest(command.BrandId, agentId, brandContractId, command.EditorUsername),
            cancellationToken);

        await repository.CreateAgentAsync(new AgentModel
        {
            AgentId = agentId,
            WalletId = walletId,
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
            EditorUsername = command.EditorUsername
        }, cancellationToken).ConfigureAwait(false);

        await repository.CreateAgentPlayerRelationAsync(command.BrandId,
            agentId,
            command.PlayerCode,
            cancellationToken);

        return agentId;
    }
}