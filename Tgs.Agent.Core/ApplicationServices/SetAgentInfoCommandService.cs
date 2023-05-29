using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.Agent.ApplicationServices;

internal class SetAgentInfoCommandService : ICommandService<SetAgentInfoCommand>
{
    private readonly IAgentRepository _agentRepository;
    private readonly ILogger<SetAgentInfoCommandService> _logger;

    public SetAgentInfoCommandService(
        IAgentRepository agentRepository,
        ILogger<SetAgentInfoCommandService> logger)
    {
        _agentRepository = agentRepository ?? throw new ArgumentNullException(nameof(agentRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async ValueTask ExecuteAsync(SetAgentInfoCommand command, CancellationToken cancellationToken = default)
    {
        var subAgent = await _agentRepository.GetAgentByIdAsync(command.SubAgentId, cancellationToken).ConfigureAwait(false);

        if (subAgent == null)
            return;

        if (subAgent.ParentAgentId != command.AgentId)
            return;

        await _agentRepository.SetAgentInfoAsync(
            command.SubAgentId,
            command.RealName,
            command.Email,
            command.Phone,
            cancellationToken);

        _logger.LogInformation(
            "Agent({agentId}) set sub agent({subAgentId}) data( RealName:{oldRealName} -> {newRealName}, Email:{oldEmail} -> {newEmail}, Phone: {oldPhone} -> {newPhone})",
            command.AgentId,
            command.SubAgentId,
            subAgent.RealName,
            command.RealName,
            subAgent.Email,
            command.Email,
            subAgent.Phone,
            command.Phone);
    }
}