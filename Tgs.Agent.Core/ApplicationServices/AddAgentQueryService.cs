using Tgs.Agent.Core.Models;
using Tgs.Core.Infrastructures.Security;
using Tgs.ServiceUtility;

namespace Tgs.Agent.Core.ApplicationServices;

internal class GetAndAddAgentQueryService : IGetService<GetAndAddAgentQuery, Guid>
{
    private readonly IAgentRepository _repository;
    private readonly IPasswordHandler _passwordHandler;

    public GetAndAddAgentQueryService(IAgentRepository repository, IPasswordHandler passwordHandler)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _passwordHandler = passwordHandler;
    }

    public async ValueTask<Guid> GetAsync(GetAndAddAgentQuery command, CancellationToken cancellationToken = default)
    {
        var (salt, pwd) = _passwordHandler.GenerateCredential(command.Password);

        return await _repository.CreateAgentAsync(new AgentModel
        {
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
    }
}