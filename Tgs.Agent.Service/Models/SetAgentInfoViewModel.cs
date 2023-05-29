namespace Tgs.Agent.Service.Models;

public class SetAgentInfoViewModel
{
    public Guid AgentId { get; set; }

    public Guid SubAgentId { get; set; }
    public string? RealName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
}