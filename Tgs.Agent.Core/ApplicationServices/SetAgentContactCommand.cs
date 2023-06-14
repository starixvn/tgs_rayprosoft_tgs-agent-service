namespace Tgs.Agent.Core.ApplicationServices;
public record SetAgentContactCommand(
    Guid AgentId,
    string? Email,
    string? Phone);