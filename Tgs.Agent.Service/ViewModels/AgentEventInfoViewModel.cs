using System.Text.Json;
using Tgs.Agent.Core;

namespace Tgs.Agent.Service.ViewModels;

public class AgentEventInfoViewModel
{
    public EventAction Action { get; set; }

    public JsonDocument ChangeSets { get; set; } = default!;

    public DateTime CreatedOn { get; set; }

    public string? Modifier { get; set; }
}