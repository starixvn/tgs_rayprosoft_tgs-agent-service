using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tgs.Agent.Core.Models.Enums;
using Tgs.ServiceUtility;

namespace Tgs.Agent.Core.ApplicationServices;

internal class SetMultipleLayerAgentInfoCommandService(
    IAgentRepository repository,
    ICommandService<AddAgentAuditLogCommand> commandService,
    ILogger<SetMultipleLayerAgentInfoCommandService> logger) : ICommandService<SetMultipleLayerAgentInfoCommand>
{
    private readonly IAgentRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public async ValueTask ExecuteAsync(SetMultipleLayerAgentInfoCommand command,
        CancellationToken cancellationToken = default)
    {
        var agent = await _repository.GetAgentByIdAsync(command.AgentId, cancellationToken)
            .ConfigureAwait(false);

        var agentPath = await _repository.GetAgentPathInfoAsync(command.AgentId, cancellationToken)
            .ConfigureAwait(false);

        if (agent is null || agentPath is null)
        {
            logger.LogWarning("Can't found agent.({agentId})", command.AgentId);

            return;
        }

        var newParentAgent = command.ParentAgentId is null
            ? null
            : await _repository.GetAgentPathInfoAsync(command.ParentAgentId.Value, cancellationToken)
                .ConfigureAwait(false);

        if (newParentAgent != null && agent.ParentAgentId != newParentAgent?.AgentId)
        {
            await _repository.UpdateSubAgentsPathAsync(agentPath, newParentAgent!.Path, cancellationToken);
        }

        var newAgentPath =
            command.ParentAgentId is not null
                ? new LTree($"{newParentAgent!.Path}.{agentPath.AgentId:N}")
                : new LTree(agentPath.Path);

        var agentParentAgentId = newParentAgent?.AgentId;

        var agentLevelType = command.ParentAgentId is not null
            ? AgentLevelType.Agent
            : AgentLevelType.Chief;

        var agentType = command.Type;

        var agentStatus = agent.ParentAgentId != newParentAgent?.AgentId
            ? AgentConfigStatus.WaitForConfigurePercentage
            : AgentConfigStatus.DoneForConfigurePercentage;

        await _repository.UpdateAgentInfoAsync(command.AgentId,
            newAgentPath,
            agentParentAgentId,
            agentLevelType,
            agentType,
            agentStatus,
            0M,
            command.NegativeAgentDividendCarryForward,
            cancellationToken);

        await _repository.UpdateAgentMultipleLayerConditionsAsync(command.AgentId,
            command.AgentDividendConditions,
            cancellationToken);

        var operation = agent.ParentAgentId != newParentAgent?.AgentId ? AgentOperation.Agent : AgentOperation.Info;

        await commandService
            .ExecuteAsync(new AddAgentAuditLogCommand(command.AgentId, command.EditorUsername, operation),
                cancellationToken)
            .ConfigureAwait(false);
    }
}