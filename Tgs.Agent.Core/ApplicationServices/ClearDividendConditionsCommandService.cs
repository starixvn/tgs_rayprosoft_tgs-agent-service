using Tgs.ServiceUtility;

namespace Tgs.Agent.Core.ApplicationServices;

internal class ClearDividendConditionsCommandService : ICommandService<ClearDividendConditionsCommand>
{
    private readonly IAgentRepository _repository;

    public ClearDividendConditionsCommandService(IAgentRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public ValueTask ExecuteAsync(ClearDividendConditionsCommand command, CancellationToken cancellationToken = default)
        => _repository.ClearAgentDividendConditionsAsync(command.AgentId, cancellationToken);
}