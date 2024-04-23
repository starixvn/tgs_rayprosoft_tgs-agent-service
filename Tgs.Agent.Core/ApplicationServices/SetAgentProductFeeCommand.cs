namespace Tgs.Agent.Core.ApplicationServices;
public record SetAgentProductFeeCommand(
    Guid AgentId,
    decimal CardGameRebatePercentage,
    decimal LiveDealerRebatePercentage,
    decimal LotteryRebatePercentage,
    decimal SlotRebatePercentage,
    decimal SportsRebatePercentage,
    decimal VirtualSportsRebatePercentage,
    string EditorUsername);