namespace Tgs.Agent.Client.Models;

public record ResetSubAgentPasswordRequest(Guid AgentId, Guid SubAgentId, string Password);