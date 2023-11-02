namespace Tgs.Agent.Client.Models;
public record GetAgentHistoriesQueryConditions(
    Guid AgentId,
    DateTime From,
    DateTime To);