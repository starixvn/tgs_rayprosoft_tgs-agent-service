namespace Tgs.Agent.Core.ApplicationServices;

public record ResetSubAgentPasswordCommand(Guid AgentId, Guid SubAgentId, string Password);