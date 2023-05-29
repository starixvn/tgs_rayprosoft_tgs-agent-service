namespace Tgs.Agent.ApplicationServices;
public record SetAgentInfoCommand(Guid AgentId, Guid SubAgentId, string? RealName, string? Email, string? Phone);