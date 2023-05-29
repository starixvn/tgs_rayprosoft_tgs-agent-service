namespace Tgs.Agent.Models;
public record AgentInfo(
    Guid Id,
    Guid? ParentAgentId,
    string? RealName,
    string? Phone,
    string? Email);