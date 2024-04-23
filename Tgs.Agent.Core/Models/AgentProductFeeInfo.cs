
namespace Tgs.Agent.Core.Models;
public record AgentProductFeeInfo(
    Guid AgentId,
    decimal CardGameRebatePercentage,
    decimal LiveDealerRebatePercentage,
    decimal LotteryRebatePercentage,
    decimal SlotRebatePercentage,
    decimal SportsRebatePercentage,
    decimal VirtualSportsRebatePercentage);