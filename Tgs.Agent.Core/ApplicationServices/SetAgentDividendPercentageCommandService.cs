using Tgs.ServiceUtility;

namespace Tgs.Agent.Core.ApplicationServices;

internal class SetAgentDividendPercentageCommandService : ICommandService<SetAgentDividendPercentageCommand>
{
    private readonly IAgentRepository _repository;

    public SetAgentDividendPercentageCommandService(IAgentRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public ValueTask ExecuteAsync(SetAgentDividendPercentageCommand command, CancellationToken cancellationToken = default)
        => _repository.SetAgentDividendPercentageAsync(command.AgentId, command.DividendPercentage, cancellationToken);
}