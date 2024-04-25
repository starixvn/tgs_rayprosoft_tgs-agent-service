using Tgs.ServiceUtility;

namespace Tgs.Agent.Core.ApplicationServices;

internal class ClearDividendConditionsCommandService(IAgentRepository repository)
    : ICommandService<ClearDividendConditionsCommand>
{
    private readonly IAgentRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public ValueTask ExecuteAsync(ClearDividendConditionsCommand command, CancellationToken cancellationToken = default)
        => _repository.ClearAgentDividendConditionsAsync(command.AgentId, cancellationToken);
}