namespace Tgs.Agent.Client.Models;

public record SetAgentPasswordRequest(Guid AgentId, string Password);