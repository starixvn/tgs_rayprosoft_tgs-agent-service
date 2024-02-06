using Microsoft.Extensions.Logging;
using Tgs.Agent.Core.Models;
using Tgs.Agent.Dividend.Client;
using Tgs.Agent.Dividend.Client.Models;
using Tgs.CashService;
using Tgs.CashService.Client;
using Tgs.Core.Infrastructures.Security;
using Tgs.ServiceUtility;

namespace Tgs.Agent.Core.ApplicationServices;

internal class AddAgentQueryService(
    IAgentRepository repository,
    IPasswordHandler passwordHandler,
    ICashServiceClient cashServiceClient,
    IAgentContractClient agentContractClient,
    IBrandContractClient brandContractClient,
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

        var brandContract = await brandContractClient.GetBrandContractAsync(command.BrandId, cancellationToken)
                                .FirstOrDefaultAsync(cancellationToken) ??
                            throw new Exception("Can't found brand contract");

        var walletId = await cashServiceClient.CreateNewCashAsync(
            command.BrandId,
            agentId.ToString(),
            command.CurrencyCode,
            CashType.Agent,
            cancellationToken).ConfigureAwait(false);

        await agentContractClient.CreateAgentContractAsync(new CreateAgentContractRequest(command.BrandId,
                agentId,
                brandContract.Id,
                command.EditorUsername),
            cancellationToken);

        await repository.CreateAgentAsync(new AgentModel
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
        }, cancellationToken).ConfigureAwait(false);

        return agentId;
    }
}