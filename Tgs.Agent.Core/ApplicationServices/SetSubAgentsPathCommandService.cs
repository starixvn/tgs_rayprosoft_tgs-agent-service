using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;

namespace Tgs.Agent.Core.ApplicationServices;

internal class SetSubAgentsPathCommandService : ICommandService<SetSubAgentsPathCommand>
{
    private readonly IAgentRepository _repository;
    private readonly ILogger<SetSubAgentsPathCommandService> _logger;

    public SetSubAgentsPathCommandService(
        IAgentRepository repository,
        ILogger<SetSubAgentsPathCommandService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async ValueTask ExecuteAsync(SetSubAgentsPathCommand command, CancellationToken cancellationToken = default)
    {
        var agentPathInfo = await _repository
            .GetAgentPathInfoAsync(command.AgentId, cancellationToken)
            .ConfigureAwait(false);

        if (agentPathInfo is null)
        {
            _logger.LogInformation("agent(agentId:{agentId}) path is null", command.AgentId);
            return;
        }

        var parentPathInfo = await _repository
            .GetAgentPathInfoAsync(command.NewParentAgentId, cancellationToken)
            .ConfigureAwait(false);

        if (parentPathInfo is null)
        {
            _logger.LogInformation("parent agent(agentId:{newParentAgentId}) path is null", command.NewParentAgentId);
            return;
        }

        await _repository
            .UpdateSubAgentsPathAsync(agentPathInfo, parentPathInfo.Path, cancellationToken)
            .ConfigureAwait(false);

        _logger.LogInformation(
            "function dbo.agent_update_path(agentId:{agentId}, agentNLevel:{agentNLevel}, agentPath:{agentPath}, parentAgentPath:{parentAgentPath}) executed",
            agentPathInfo.AgentId,
            agentPathInfo.NLevel,
            agentPathInfo.Path,
            parentPathInfo.Path);
    }
}