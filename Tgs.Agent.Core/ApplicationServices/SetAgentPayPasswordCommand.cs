namespace Tgs.Agent.Core.ApplicationServices;
public record SetAgentPayPasswordCommand(Guid AgentId, string PayPassword);