namespace Tgs.Agent.Service.Models;

public class ResetSubAgentPasswordViewModel
{
    public Guid AgentId { get; set; }
    public Guid SubAgentId { get; set; }
    public string Password { get; set; } = default!;
}