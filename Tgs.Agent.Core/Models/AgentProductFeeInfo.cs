
namespace Tgs.Agent.Core.Models;
public record AgentProductFeeInfo(
    Guid AgentId,
    decimal CardGamePercentage,
    decimal LiveDealerPercentage,
    decimal LotteryPercentage,
    decimal SlotPercentage,
    decimal SportsPercentage,
    decimal VirtualSportPercentage);