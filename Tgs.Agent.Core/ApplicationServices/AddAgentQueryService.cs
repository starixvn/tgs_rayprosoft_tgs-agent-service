using Tgs.Agent.Core.Models;
using Tgs.CashService;
using Tgs.CashService.Client;
using Tgs.Core.Infrastructures.Security;
using Tgs.ServiceUtility;

namespace Tgs.Agent.Core.ApplicationServices;

internal class AddAgentQueryService : IGetService<AddAgentQuery, Guid>
{
    private readonly IAgentRepository _repository;
    private readonly IPasswordHandler _passwordHandler;
    private readonly ICashServiceClient _cashServiceClient;

    public AddAgentQueryService(IAgentRepository repository, IPasswordHandler passwordHandler, ICashServiceClient cashServiceClient)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _passwordHandler = passwordHandler;
        _cashServiceClient = cashServiceClient;
    }

    public async ValueTask<Guid> GetAsync(AddAgentQuery command, CancellationToken cancellationToken = default)
    {
        var (salt, pwd) = _passwordHandler.GenerateCredential(command.Password);

        var agentId = Guid.NewGuid();

        var walletId = await _cashServiceClient.CreateNewCashAsync(command.BrandId,
            agentId.ToString(),
            command.CurrencyCode,
            CashType.Agent,
            cancellationToken);

        await _repository.CreateAgentAsync(new AgentModel
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
            EditorUsername = command.EditorUsername
        }, cancellationToken);

        return agentId;
    }
}