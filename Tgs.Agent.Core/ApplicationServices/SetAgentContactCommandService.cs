using Tgs.ServiceUtility;

namespace Tgs.Agent.Core.ApplicationServices;

internal class SetAgentContactCommandService(IAgentRepository repository) : ICommandService<SetAgentContactCommand>
{
    private readonly IAgentRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public ValueTask ExecuteAsync(SetAgentContactCommand command, CancellationToken cancellationToken = default)
        => _repository.SetAgentContactAsync(command.AgentId, command.Email, command.Phone, cancellationToken);
}