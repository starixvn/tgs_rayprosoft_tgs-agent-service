using Tgs.Agent.Core;

namespace Tgs.Agent.Service.ViewModels;

public record AgentHistoryInfoViewModel(
    Guid EventId,
    EventAction Action,
    string? Modifier,
    DateTimeOffset CreatedOn);