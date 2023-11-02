using Tgs.Agent.Client.Models.Enums;

namespace Tgs.Agent.Client.Models;
public record AgentHistoryInfo
{
    public EventAction Action { get; set; } = default!;

    public DateTimeOffset CreatedOn { get; set; }

    public Guid EventId { get; set; }

    public string? Modifier { get; set; }
}