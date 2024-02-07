using Tgs.Agent.Core.Models.Enums;

namespace Tgs.Agent.Core.Models;

public record UpdateAgentModel(
    Guid AgentId,
    Guid? ParentAgentId,
    AgentStatus AgentStatus,
    AgentLevelType AgentLevelType,
    AgentConfigStatus AgentConfigStatus,
    bool NegativeAgentDividendCarryForward,
    string? RealName,
    string? Remarks,
    string EditorUsername);