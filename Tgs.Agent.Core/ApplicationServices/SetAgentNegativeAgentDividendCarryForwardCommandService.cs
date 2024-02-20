using Tgs.ServiceUtility;

namespace Tgs.Agent.Core.ApplicationServices;

internal class SetAgentNegativeAgentDividendCarryForwardCommandService(
    IAgentRepository agentRepository)
    : ICommandService<SetAgentNegativeAgentDividendCarryForwardCommand>
{
    public ValueTask ExecuteAsync(SetAgentNegativeAgentDividendCarryForwardCommand command,
        CancellationToken cancellationToken = default)
    {
        return agentRepository.UpdateAgentNegativeAgentDividendCarryForwardAsync(
            command.AgentId,
            command.NegativeAgentDividendCarryForward,
            cancellationToken);
    }
}