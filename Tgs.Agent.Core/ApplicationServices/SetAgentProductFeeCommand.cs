namespace Tgs.Agent.Core.ApplicationServices;
public record SetAgentProductFeeCommand(
    Guid AgentId,
    decimal CardGamePercentage,
    decimal LiveDealerPercentage,
    decimal LotteryPercentage,
    decimal SlotPercentage,
    decimal SportsPercentage,
    decimal VirtualSportsPercentage,
    string EditorUsername);