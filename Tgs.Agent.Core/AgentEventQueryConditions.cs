namespace Tgs.Agent.Core;
public record AgentEventQueryConditions(
    Guid AgentId,
    DateTime From,
    DateTime To);