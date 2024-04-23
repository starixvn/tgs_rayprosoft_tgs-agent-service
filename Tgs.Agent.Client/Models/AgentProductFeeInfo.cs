
namespace Tgs.Agent.Client.Models;
public record AgentProductFeeInfo(
    Guid AgentId,
    decimal CardGameRebatePercentage,
    decimal LiveDealerRebatePercentage,
    decimal LotteryRebatePercentage,
    decimal SlotRebatePercentage,
    decimal SportsRebatePercentage,
    decimal VirtualSportsRebatePercentage);