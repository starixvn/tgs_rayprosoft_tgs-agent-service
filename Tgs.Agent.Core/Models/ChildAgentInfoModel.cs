namespace Tgs.Agent.Core.Models;

public class ChildAgentInfoModel
{
    public Guid AgentId { get; set; }
    public string AgentUsername { get; set; } = default!;
    public Guid ParentAgentId { get; set; }
}