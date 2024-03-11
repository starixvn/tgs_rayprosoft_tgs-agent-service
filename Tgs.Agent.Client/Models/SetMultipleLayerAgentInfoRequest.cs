using Tgs.Agent.Client.Models.Enums;

namespace Tgs.Agent.Client.Models;
public record SetMultipleLayerAgentInfoRequest(
    Guid AgentId,
    Guid? ParentAgentId,
    decimal DividendPercentage,
    AgentType Type,
    bool NegativeAgentDividendCarryForward,
    AgentDividendConditionModel[] AgentDividendConditions,
    string EditorUsername);