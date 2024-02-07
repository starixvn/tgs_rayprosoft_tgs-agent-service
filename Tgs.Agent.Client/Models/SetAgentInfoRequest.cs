using Tgs.Agent.Client.Models.Enums;

namespace Tgs.Agent.Client.Models;
public record SetAgentInfoRequest(
    Guid AgentId,
    Guid? ParentAgentId,
    AgentStatus AgentStatus,
    AgentLevelType AgentLevelType,
    AgentConfigStatus AgentConfigStatus,
    bool NegativeAgentDividendCarryForward,
    string? RealName,
    string? Remarks,
    string EditorUsername);