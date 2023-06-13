using Tgs.ServiceUtility;

namespace Tgs.Agent.Core.ApplicationServices;

internal class AddAgentAuditLogCommandService : ICommandService<AddAgentAuditLogCommand>
{
    private readonly IAgentRepository _repository;

    public AddAgentAuditLogCommandService(IAgentRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public ValueTask ExecuteAsync(AddAgentAuditLogCommand command, CancellationToken cancellationToken = default)
        => _repository.AddAgentAuditLogAsync(
            command.AgentId,
            command.Operation,
            command.EditUsername,
            cancellationToken);
}