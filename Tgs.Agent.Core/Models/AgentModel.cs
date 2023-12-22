namespace Tgs.Agent.Core.Models;

public class AgentModel
{
    public Guid AgentId { get; set; }
    public Guid BrandId { get; init; }
    public Guid WalletId { get; set; }
    public Guid? ParentAgentId { get; init; }
    public string Username { get; init; } = default!;
    public AgentLevelType LevelType { get; init; }
    public AgentType Type { get; init; }
    public decimal CardGameRebatePercentage { get; init; }
    public decimal CommissionPercentage { get; init; }
    public decimal DividendPercentage { get; init; }
    public decimal LiveDealerRebatePercentage { get; init; }
    public decimal LotteryRebatePercentage { get; init; }
    public byte[] Password { get; init; } = default!;
    public byte[] Salt { get; init; } = default!;
    public string RealName { get; init; } = default!;

    public string Email { get; init; } = default!;

    public string Phone { get; init; } = default!;
    public string Remarks { get; init; } = default!;
    public decimal SlotRebatePercentage { get; init; }
    public decimal SportsRebatePercentage { get; init; }
    public decimal VirtualSportsRebatePercentage { get; init; }
    public string EditorUsername { get; set; } = default!;
}