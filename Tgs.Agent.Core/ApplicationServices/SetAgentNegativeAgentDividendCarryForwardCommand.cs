
namespace Tgs.Agent.Core.ApplicationServices;
public record SetAgentNegativeAgentDividendCarryForwardCommand(
    Guid AgentId,
    bool NegativeAgentDividendCarryForward,
    string? ModifyUserId);
