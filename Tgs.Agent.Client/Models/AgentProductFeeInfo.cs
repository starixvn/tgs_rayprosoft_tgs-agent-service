
namespace Tgs.Agent.Client.Models;
public record AgentProductFeeInfo(
    Guid AgentId,
    decimal CardGamePercentage,
    decimal LiveDealerPercentage,
    decimal LotteryPercentage,
    decimal SlotPercentage,
    decimal SportsPercentage,
    decimal VirtualSportsPercentage);