
namespace Tgs.Agent.Service.ViewModels;

public class SetAgentProductFeeViewModel
{
    public decimal CardGamePercentage { get; set; }
    public decimal LiveDealerPercentage { get; set; }
    public decimal LotteryPercentage { get; set; }
    public decimal SlotPercentage { get; set; }
    public decimal SportsPercentage { get; set; }
    public decimal VirtualSportsPercentage { get; set; }
    public required string EditorUsername { get; set; }
}