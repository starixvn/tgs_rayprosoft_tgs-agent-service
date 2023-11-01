namespace Tgs.Agent.Core.ApplicationServices;
public record GetAgentHistoriesQuery(
    Guid AgentId,
    DateTime From,
    DateTime To,
    int Skip = 0);