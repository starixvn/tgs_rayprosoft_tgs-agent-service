using Tgs.Agent.Core.Models;
using Tgs.Agent.Core.Models.Enums;

namespace Tgs.Agent.Service.ViewModels;

public class UpdateMultipleLayerAgentInfoViewModel
{
    public Guid? ParentAgentId { get; set; }
    public AgentType Type { get; set; }
    public AgentDividendConditionModel[] AgentDividendConditions { get; set; } = [];
    public string EditorUsername { get; set; } = string.Empty;
}