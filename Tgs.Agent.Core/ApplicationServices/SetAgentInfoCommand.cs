using Tgs.Agent.Core.Models.Enums;

namespace Tgs.Agent.Core.ApplicationServices;
public record SetAgentInfoCommand(
    Guid AgentId,
    Guid? ParentAgentId,
    AgentStatus AgentStatus,
    AgentLevelType AgentLevelType,
    AgentConfigStatus AgentConfigStatus,
    bool NegativeAgentDividendCarryForward, 
    string? RealName,
    string? Remarks,
    string EditorUsername);