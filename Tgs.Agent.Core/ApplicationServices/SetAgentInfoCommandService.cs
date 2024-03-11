using Tgs.ServiceUtility;

namespace Tgs.Agent.Core.ApplicationServices;

internal class SetAgentInfoCommandService(IAgentRepository repository) : ICommandService<SetAgentInfoCommand>
{
    private readonly IAgentRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public ValueTask ExecuteAsync(SetAgentInfoCommand command, CancellationToken cancellationToken = default)
        => _repository.SetAgentInfoAsync(command.AgentId,
            command.RealName,
            cancellationToken);
}