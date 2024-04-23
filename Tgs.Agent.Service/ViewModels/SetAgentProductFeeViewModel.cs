
namespace Tgs.Agent.Service.ViewModels;

public class SetAgentProductFeeViewModel
{
    public decimal CardGameRebatePercentage { get; set; }
    public decimal LiveDealerRebatePercentage { get; set; }
    public decimal LotteryRebatePercentage { get; set; }
    public decimal SlotRebatePercentage { get; set; }
    public decimal SportsRebatePercentage { get; set; }
    public decimal VirtualSportsRebatePercentage { get; set; }
    public required string EditorUsername { get; set; }
}