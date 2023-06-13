using Tgs.Agent.Core;
using Tgs.Agent.Core.Models.Enums;
using Tgs.Agent.Repositories.Entities;

namespace Tgs.Agent.Repositories;

internal class AgentAuditLogRepository : IAgentAuditLogRepository
{
    private readonly TgsContext _tgsContext;

    public AgentAuditLogRepository(TgsContext tgsContext)
    {
        _tgsContext = tgsContext ?? throw new ArgumentNullException(nameof(tgsContext));
    }

    public async ValueTask AddAgentAuditLogAsync(Guid agentId, AgentOperation operation, string editUsername, CancellationToken cancellationToken = default)
    {
        var auditLog = new AgentAuditLog
        {
            Id = Guid.NewGuid(),
            AgentId = agentId,
            CreatedByUsername = editUsername,
            CreatedOn = DateTime.UtcNow,
            Operation = (short)operation
        };

        _ = await _tgsContext.AgentAuditLogs
            .AddAsync(auditLog, cancellationToken)
            .ConfigureAwait(false);

        _ = await _tgsContext
            .SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}