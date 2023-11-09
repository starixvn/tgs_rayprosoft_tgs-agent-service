namespace Tgs.Agent.Core.ApplicationServices;
public record SetSubAgentsPathCommand(
    Guid AgentId,
    Guid NewParentAgentId);