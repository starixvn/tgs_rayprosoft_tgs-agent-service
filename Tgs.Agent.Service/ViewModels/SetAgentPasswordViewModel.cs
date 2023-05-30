namespace Tgs.Agent.Service.ViewModels;

public class SetAgentPasswordViewModel
{
    public Guid AgentId { get; set; }
    public string Password { get; set; } = default!;
}