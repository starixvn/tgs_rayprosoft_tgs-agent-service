
namespace Tgs.Agent.Core.Models;
public record AgentProductFeeInfo(
    Guid AgentId,
    decimal PreviousCardGamePercentage,
    decimal PreviousLiveDealerPercentage,
    decimal PreviousLotteryPercentage,
    decimal PreviousSlotPercentage,
    decimal PreviousSportsPercentage,
    decimal PreviousVirtualSportsPercentage,
    decimal CurrentCardGamePercentage,
    decimal CurrentLiveDealerPercentage,
    decimal CurrentLotteryPercentage,
    decimal CurrentSlotPercentage,
    decimal CurrentSportsPercentage,
    decimal CurrentVirtualSportsPercentage,
    decimal NextCardGamePercentage,
    decimal NextLiveDealerPercentage,
    decimal NextLotteryPercentage,
    decimal NextSlotPercentage,
    decimal NextSportsPercentage,
    decimal NextVirtualSportsPercentage);