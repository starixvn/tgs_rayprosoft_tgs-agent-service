namespace Tgs.Agent.Core.ApplicationServices;

public record SetAgentPasswordCommand(
    Guid AgentId, 
    string Password,
    string? ModifyUserId);