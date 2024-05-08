using Tgs.Agent.Core.Models;
using Tgs.Agent.Core.Models.Enums;

namespace Tgs.Agent.Core.ApplicationServices;
public record SetMultipleLayerAgentInfoCommand(
    Guid AgentId,
    Guid? ParentAgentId,
    AgentType Type,
    AgentDividendConditionModel[] AgentDividendConditions,
    string EditorUsername);