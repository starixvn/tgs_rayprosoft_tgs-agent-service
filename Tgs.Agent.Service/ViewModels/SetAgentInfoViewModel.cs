using Tgs.Agent.Core.Models.Enums;

namespace Tgs.Agent.Service.ViewModels;

public class SetAgentInfoViewModel
{
    public Guid? ParentAgentId { get; set; }
    public AgentStatus AgentStatus { get; set; }
    public AgentLevelType AgentLevelType { get; set; }
    public AgentConfigStatus AgentConfigStatus { get; set; }
    public bool NegativeAgentDividendCarryForward { get; set; }
    public string? RealName { get; set; }
    public string? Remarks { get; set; }
    public string EditorUsername { get; set; } = string.Empty;
}