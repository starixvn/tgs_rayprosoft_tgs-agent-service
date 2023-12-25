namespace Tgs.Agent.Core.ApplicationServices;

public record AddAgentQuery
{
    public string Username { get; init; } = default!;
    public Guid BrandId { get; init; }
    public Guid? ParentAgentId { get; init; }
    public AgentLevelType LevelType { get; init; }
    public AgentType Type { get; init; }
    public string CurrencyCode { get; init; } = default!;
    public decimal CardGameRebatePercentage { get; init; }
    public decimal CommissionPercentage { get; init; }
    public decimal DividendPercentage { get; init; }
    public decimal LiveDealerRebatePercentage { get; init; }
    public decimal LotteryRebatePercentage { get; init; }
    public string Password { get; init; } = default!;
    public string RealName { get; init; } = default!;
    public string Remarks { get; init; } = default!;
    public decimal SlotRebatePercentage { get; init; }
    public decimal SportsRebatePercentage { get; init; }
    public decimal VirtualSportsRebatePercentage { get; init; }
    public decimal GradeIntervalLimit { get; init; }
    public string EditorUsername { get; set; } = default!;
}