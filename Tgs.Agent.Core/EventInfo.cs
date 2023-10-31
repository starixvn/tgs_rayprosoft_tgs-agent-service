using System.Text.Json;

namespace Tgs.Agent.Core;
public record EventInfo(
    EventAction Action,
    JsonDocument ChangeSets,
    string? Modifier,
    DateTime CreatedOn);