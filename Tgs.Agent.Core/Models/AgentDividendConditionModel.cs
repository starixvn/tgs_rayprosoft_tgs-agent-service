
namespace Tgs.Agent.Core.Models;

public record AgentDividendConditionModel(
    int Rank,
    int ActiveNumber,
    decimal NetProfit,
    decimal DividendPercentage);