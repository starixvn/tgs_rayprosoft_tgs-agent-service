
namespace Tgs.Agent.Client.Models;

public record AgentDividendConditionModel(
    int Rank,
    int ActiveNumber,
    decimal NetProfit,
    decimal DividendPercentage);