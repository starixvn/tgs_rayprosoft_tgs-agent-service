using Tgs.ServiceUtility;

namespace Tgs.Agent.Core.ApplicationServices;

internal class SetAgentContactCommandService : ICommandService<SetAgentContactCommand>
{
    private readonly IAgentRepository _repository;

    public SetAgentContactCommandService(IAgentRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public ValueTask ExecuteAsync(SetAgentContactCommand command, CancellationToken cancellationToken = default)
        => _repository.SetAgentContactAsync(command.AgentId, command.Email, command.Phone, cancellationToken);
}