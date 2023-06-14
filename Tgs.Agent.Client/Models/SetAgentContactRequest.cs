namespace Tgs.Agent.Client.Models;
public record SetAgentContactRequest(
    Guid AgentId,
    string? Email,
    string? Phone);