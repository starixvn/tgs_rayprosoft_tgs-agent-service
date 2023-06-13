namespace Tgs.Agent.Service.ViewModels;

public class ChildAgentInfoViewModel
{
    public Guid AgentId { get; set; }
    public string AgentUsername { get; set; } = default!;
    public Guid ParentAgentId { get; set; }
    public decimal DividendPercentage { get; set; }
}