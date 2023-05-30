namespace Tgs.Agent.Core.ApplicationServices;
public record SetAgentInfoCommand(
    Guid AgentId,
    string? RealName,
    string? Email,
    string? Phone);