namespace Tgs.Agent.Client.Models;
public record SetAgentInfoRequest(
    Guid AgentId,
    string? RealName,
    string? Email,
    string? Phone);