using Tgs.Agent.Core.Models.Enums;

namespace Tgs.Agent.Core;

public interface IAgentAuditLogRepository
{
    public ValueTask AddAgentAuditLogAsync(Guid agentId, AgentOperation operation, string editUsername, CancellationToken cancellationToken = default);
}