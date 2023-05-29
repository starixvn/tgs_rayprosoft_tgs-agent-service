using Microsoft.Extensions.Logging;
using Tgs.Core.Infrastructures.Security;
using Tgs.ServiceUtility;

namespace Tgs.Agent.Core.ApplicationServices;

internal class ResetSubAgentPasswordCommandService : ICommandService<ResetSubAgentPasswordCommand>
{
    private readonly IAgentRepository _agentRepository;
    private readonly IPasswordHandler _passwordHandler;
    private readonly ILogger<ResetSubAgentPasswordCommandService> _logger;

    public ResetSubAgentPasswordCommandService(
        IAgentRepository agentRepository,
        IPasswordHandler passwordHandler,
        ILogger<ResetSubAgentPasswordCommandService> logger)
    {
        _agentRepository = agentRepository ?? throw new ArgumentNullException(nameof(agentRepository));
        _passwordHandler = passwordHandler ?? throw new ArgumentNullException(nameof(passwordHandler));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async ValueTask ExecuteAsync(ResetSubAgentPasswordCommand command, CancellationToken cancellationToken = default)
    {
        var subAgent = await _agentRepository
            .GetAgentByIdAsync(command.SubAgentId, cancellationToken)
            .ConfigureAwait(false);

        if (subAgent == null)
            return;

        if (subAgent.ParentAgentId != command.AgentId)
            return;

        var (salt, hashedPassword) = _passwordHandler.GenerateCredential(command.Password);

        await _agentRepository.SetPasswordAsync(
            command.SubAgentId,
            hashedPassword,
            salt,
            cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("Agent({agentId}) reset sub agent({subAgentId}) password",
            command.AgentId,
            command.SubAgentId);
    }
}