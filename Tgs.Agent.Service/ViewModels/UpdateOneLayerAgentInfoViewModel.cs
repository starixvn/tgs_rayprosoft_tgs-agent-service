using Tgs.Agent.Core.Models.Enums;

namespace Tgs.Agent.Service.ViewModels;

public class UpdateOneLayerAgentInfoViewModel
{
    public Guid? ParentAgentId { get; set; }
    public decimal DividendPercentage { get; set; }
    public AgentType Type { get; set; }
    public string EditorUsername { get; set; } = string.Empty;
}