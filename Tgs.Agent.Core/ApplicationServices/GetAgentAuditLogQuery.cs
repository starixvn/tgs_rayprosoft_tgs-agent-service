using Tgs.Paging;

namespace Tgs.Agent.Core.ApplicationServices;
public record GetAgentAuditLogQuery(
    Guid BrandId,
    Guid AgentId,
    DateTimeOffset From,
    DateTimeOffset To,
    Paging<AgentAuditLogInfo> Paging);