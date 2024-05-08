using Tgs.Agent.Client.Models.Enums;

namespace Tgs.Agent.Client.Models;
public record SetMultipleLayerAgentInfoRequest(
    Guid AgentId,
    Guid? ParentAgentId,
    AgentType Type,
    AgentDividendConditionModel[] AgentDividendConditions,
    string EditorUsername);