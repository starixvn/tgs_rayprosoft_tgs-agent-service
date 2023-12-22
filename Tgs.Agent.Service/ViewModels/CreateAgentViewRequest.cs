using Tgs.Agent.Core;

namespace Tgs.Agent.Service.ViewModels;

public class CreateAgentViewRequest
{
    public Guid BrandId { get; init; }
    public Guid? ParentAgentId { get; init; }
    public string Username { get; init; } = null!;
    public string CurrencyCode { get; init; } = null!;
    public AgentLevelType LevelType { get; init; }
    public AgentType Type { get; init; }
    public string Password { get; init; } = null!;
    public string RealName { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string Phone { get; init; } = null!;
    public string Remarks { get; init; } = null!;
    public decimal CardGameRebatePercentage { get; init; }
    public decimal CommissionPercentage { get; init; }
    public decimal DividendPercentage { get; init; }
    public decimal LiveDealerRebatePercentage { get; init; }
    public decimal LotteryRebatePercentage { get; init; }
    public decimal SlotRebatePercentage { get; init; }
    public decimal SportsRebatePercentage { get; init; }
    public decimal VirtualSportsRebatePercentage { get; init; }
    public string EditorUsername { get; set; } = null!;
}