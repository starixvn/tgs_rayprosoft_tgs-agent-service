namespace Tgs.Agent.Core.ApplicationServices;
public record SetAgentDividendPercentageCommand(Guid AgentId, decimal DividendPercentage);