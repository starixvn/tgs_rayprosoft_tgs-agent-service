using System.Collections.Immutable;

namespace Tgs.Agent.Core.ApplicationServices;
public record SetAgentPropertiesCommand(
    Guid AgentId,
    IImmutableDictionary<string, string?> Properties,
    string? ModifyUserId);