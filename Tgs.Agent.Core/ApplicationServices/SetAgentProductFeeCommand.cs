namespace Tgs.Agent.Core.ApplicationServices;
public record SetAgentProductFeeCommand(
    Guid AgentId,
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
    decimal NextVirtualSportsPercentage,
    string EditorUsername);