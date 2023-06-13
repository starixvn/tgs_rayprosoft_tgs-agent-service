using Tgs.ServiceUtility;

namespace Tgs.Agent.Core.ApplicationServices;

internal class AddAgentAuditLogCommandService : ICommandService<AddAgentAuditLogCommand>
{
    private readonly IAgentAuditLogRepository _repository;

    public AddAgentAuditLogCommandService(IAgentAuditLogRepository repository)
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