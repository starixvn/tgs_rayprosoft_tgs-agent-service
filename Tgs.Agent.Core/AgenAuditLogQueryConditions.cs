namespace Tgs.Agent.Core;
public record AgenAuditLogQueryConditions(
    Guid BrandId,
    Guid AgentId,
    DateTimeOffset From,
    DateTimeOffset To);