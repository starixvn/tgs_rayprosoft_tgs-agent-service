using Tgs.Agent.Core.Models.Enums;

namespace Tgs.Agent.Core.Models;

public class AgentModel
{
    public string Username { get; init; } = default!;
    public Guid? ParentAgentId { get; init; }
    public Guid BrandId { get; init; }
    public decimal CardGameRebatePercentage { get; init; }
    public decimal CommissionPercentage { get; init; }
    //佔成
    public decimal DividendPercentage { get; init; }
    public AgentLevelType LevelType { get; init; }
    public decimal LiveDealerRebatePercentage { get; init; }
    public decimal LotteryRebatePercentage { get; init; }
    public byte[] Password { get; init; } = default!;
    public byte[] Salt { get; init; } = default!;
    public string RealName { get; init; } = default!;
    public string Remarks { get; init; } = default!;
    public decimal SlotRebatePercentage { get; init; }
    public decimal SportsRebatePercentage { get; init; }
    public AgentType Type { get; init; }
    public decimal VirtualSportsRebatePercentage { get; init; }
    public string EditorUsername { get; set; } = default!;
}
