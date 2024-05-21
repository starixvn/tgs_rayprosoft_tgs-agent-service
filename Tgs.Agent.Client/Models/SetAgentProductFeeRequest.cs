
namespace Tgs.Agent.Client.Models;

public class SetAgentProductFeeRequest
{
    public Guid AgentId { get; set; }
    public decimal CurrentCardGamePercentage { get; set; }
    public decimal CurrentLiveDealerPercentage { get; set; }
    public decimal CurrentLotteryPercentage { get; set; }
    public decimal CurrentSlotPercentage { get; set; }
    public decimal CurrentSportsPercentage { get; set; }
    public decimal CurrentVirtualSportsPercentage { get; set; }
    public decimal NextCardGamePercentage { get; set; }
    public decimal NextLiveDealerPercentage { get; set; }
    public decimal NextLotteryPercentage { get; set; }
    public decimal NextSlotPercentage { get; set; }
    public decimal NextSportsPercentage { get; set; }
    public decimal NextVirtualSportsPercentage { get; set; }
    public required string EditorUsername { get; set; }
}