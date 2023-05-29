namespace Tgs.Agent.Client.Models;
public record SetSubAgentInfoRequest(Guid AgentId, Guid SubAgentId, string? RealName, string? Email, string? Phone);