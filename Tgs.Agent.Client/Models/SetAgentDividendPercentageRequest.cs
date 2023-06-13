namespace Tgs.Agent.Client.Models;
public record SetAgentDividendPercentageRequest(
    Guid AgentId,
    decimal DividendPercentage);