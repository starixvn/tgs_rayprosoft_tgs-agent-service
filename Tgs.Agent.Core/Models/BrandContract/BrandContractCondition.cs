namespace Tgs.Agent.Core.Models.BrandContract
{
    public record BrandContractCondition(
        int Rank,
        int ActiveNumber,
        decimal DividendPercentage,
        decimal NetProfit,
        decimal WinLossAmount
        );
}
