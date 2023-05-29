namespace Tgs.Agent.Service.ViewModels;

public class ResetSubAgentPasswordViewModel
{
    public Guid AgentId { get; set; }
    public Guid SubAgentId { get; set; }
    public string Password { get; set; } = default!;
}
