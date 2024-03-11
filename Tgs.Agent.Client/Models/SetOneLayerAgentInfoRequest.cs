using Tgs.Agent.Client.Models.Enums;

namespace Tgs.Agent.Client.Models;
public record SetOneLayerAgentInfoRequest(
    Guid AgentId,
    Guid? ParentAgentId,
    decimal DividendPercentage,
    AgentType Type,
    bool NegativeAgentDividendCarryForward,
    string EditorUsername);