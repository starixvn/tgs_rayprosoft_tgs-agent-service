namespace Tgs.Agent.Core;
public record AgentHistoryInfo(
    Guid EventId,
    EventAction Action,
    string? Modifier,
    DateTimeOffset CreatedOn);