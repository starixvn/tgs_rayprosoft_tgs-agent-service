using Microsoft.Extensions.Logging;
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
    private readonly ILogger<AddAgentQueryService> _logger;

    public AddAgentQueryService(
        IAgentRepository repository,
        IPasswordHandler passwordHandler,
        ICashServiceClient cashServiceClient,
        ILogger<AddAgentQueryService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _passwordHandler = passwordHandler;
        _cashServiceClient = cashServiceClient;
        _logger = logger;
    }

    public async ValueTask<Guid> GetAsync(AddAgentQuery command, CancellationToken cancellationToken = default)
    {
        var (salt, pwd) = _passwordHandler.GenerateCredential(command.Password);

        var agentId = Guid.NewGuid();

        _logger.LogInformation("brand:{BrandId}, agent:{Agent}, currencyCode:{CurrencyCode}",
            command.BrandId,
            agentId,
            command.CurrencyCode);

        var walletId = await _cashServiceClient.CreateNewCashAsync(
            command.BrandId,
            agentId.ToString(),
            command.CurrencyCode,
            CashType.Agent,
            cancellationToken).ConfigureAwait(false);

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
            Email = command.Email,
            Phone = command.Phone,
            EditorUsername = command.EditorUsername
        }, cancellationToken).ConfigureAwait(false);

        return agentId;
    }
}