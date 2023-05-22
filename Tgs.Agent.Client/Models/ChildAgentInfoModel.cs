namespace Tgs.Agent.Client.Models;

public class ChildAgentInfoModel
{
    public Guid AgentId { get; set; }
    public Guid ParentAgentId { get; set; }
}