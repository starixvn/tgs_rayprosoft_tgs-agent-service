using System.Text.Json;
using Tgs.Agent.Client.Models.Enums;

namespace Tgs.Agent.Client.Models;
public record AgentEventInfo
{
    public EventAction Action { get; set; }

    public JsonDocument ChangeSets { get; set; } = default!;

    public DateTime CreatedOn { get; set; }

    public string? Modifier { get; set; }
}