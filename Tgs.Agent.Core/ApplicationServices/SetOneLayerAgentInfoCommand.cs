using Tgs.Agent.Core.Models.Enums;

namespace Tgs.Agent.Core.ApplicationServices;
public record SetOneLayerAgentInfoCommand(
    Guid AgentId,
    Guid? ParentAgentId,
    decimal DividendPercentage,
    AgentType Type,
    bool NegativeAgentDividendCarryForward,
    string EditorUsername);