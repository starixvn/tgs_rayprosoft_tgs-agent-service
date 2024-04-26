using Json.Patch;
using Json.Pointer;
using System.Text.Json.Nodes;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tgs.Agent.Core.Models.Enums;
using Tgs.SerialNumberGenerator;
using Tgs.ServiceUtility;

namespace Tgs.Agent.Core.ApplicationServices;

internal class SetOneLayerAgentInfoCommandService(
    IAgentRepository repository,
    ICommandService<AddAgentAuditLogCommand> commandService,
    ISerialNumberGenerator serialNumberGenerator,
    ILogger<SetOneLayerAgentInfoCommandService> logger) : ICommandService<SetOneLayerAgentInfoCommand>
{
    private readonly IAgentRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public async ValueTask ExecuteAsync(SetOneLayerAgentInfoCommand command,
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
            var agents = await _repository.GetChildAgentInfosAsync(command.AgentId, cancellationToken)
                .ToArrayAsync(cancellationToken)
                .ConfigureAwait(false);

            await _repository.ResetSubAgentDividendPercentageAsync(agents.Select(x => x.AgentId).ToArray(),
                cancellationToken);

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

        var agentDividendPercentage = command.DividendPercentage;

        var agentStatus = agent.ParentAgentId != newParentAgent?.AgentId
            ? AgentConfigStatus.WaitForConfigurePercentage
            : AgentConfigStatus.DoneForConfigurePercentage;

        await _repository.UpdateAgentInfoAsync(command.AgentId,
            newAgentPath,
            agentParentAgentId,
            agentLevelType,
            agentType,
            agentStatus,
            agentDividendPercentage,
            command.NegativeAgentDividendCarryForward,
            cancellationToken);

        var operation = agent.ParentAgentId != newParentAgent?.AgentId ? AgentOperation.Agent : AgentOperation.Info;

        await commandService
            .ExecuteAsync(new AddAgentAuditLogCommand(command.AgentId, command.EditorUsername, operation),
                cancellationToken)
            .ConfigureAwait(false);

        var eventId = await serialNumberGenerator
            .GenerateAsync(cancellationToken)
            .ConfigureAwait(false);

        var patchOperations = Array.Empty<PatchOperation>();

        patchOperations =
        [
            .. patchOperations,
            PatchOperation.Replace(JsonPointer.Parse("/username"),
                JsonNode.Parse(JsonSerializer.Serialize(agent.Username, AgentUtility.SerializerOptions))),
            PatchOperation.Replace(JsonPointer.Parse("/type"),
                JsonNode.Parse(JsonSerializer.Serialize(agent.Type.ToString(), AgentUtility.SerializerOptions))),
            PatchOperation.Replace(JsonPointer.Parse("/levelType"),
                JsonNode.Parse(JsonSerializer.Serialize(agentLevelType.ToString(), AgentUtility.SerializerOptions))),
            PatchOperation.Replace(JsonPointer.Parse("/dividendPercentage"),
                JsonNode.Parse(JsonSerializer.Serialize(agentDividendPercentage, AgentUtility.SerializerOptions))),
        ];

        var changeSets = new JsonPatch(patchOperations);

        await repository.CreateAgentEventAsync(
            eventId,
            agent.BrandId,
            agent.Id,
            EventAction.Create,
            JsonSerializer.SerializeToDocument(changeSets, AgentUtility.SerializerOptions),
            command.EditorUsername,
            cancellationToken).ConfigureAwait(false);
    }
}