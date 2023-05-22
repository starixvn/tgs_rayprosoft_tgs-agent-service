namespace Tgs.Agent.Core.Models;

public class ChildAgentInfoModel
{
    public Guid AgentId { get; set; }
    public Guid ParentAgentId { get; set; }
}