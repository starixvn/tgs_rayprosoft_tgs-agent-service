namespace Tgs.Agent.Client.Models;
public record SetAgentInfoRequest(Guid AgentId, Guid SubAgentId, string? RealName, string? Email, string? Phone);