using System.Text.Json;
using Json.Patch;
using Json.Pointer;
using Tgs.SerialNumberGenerator;
using Tgs.ServiceUtility;

namespace Tgs.Agent.Core.ApplicationServices;

internal class SetAgentStatusCommandService : ICommandService<SetAgentStatusCommand>
{
    private readonly IAgentRepository _agentRepository;
    private readonly ISystemClock _systemClock;
    private readonly ISerialNumberGenerator _serialNumberGenerator;

    public SetAgentStatusCommandService(
        IAgentRepository agentRepository,
        ISystemClock systemClock,
        ISerialNumberGenerator serialNumberGenerator)
    {
        _agentRepository = agentRepository ?? throw new ArgumentNullException(nameof(agentRepository));
        _systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
        _serialNumberGenerator = serialNumberGenerator ?? throw new ArgumentNullException(nameof(serialNumberGenerator));
    }

    public async ValueTask ExecuteAsync(SetAgentStatusCommand command, CancellationToken cancellationToken = default)
    {        
        var patchOperations = Array.Empty<PatchOperation>();

        patchOperations = patchOperations.Append(PatchOperation.Replace(JsonPointer.Parse("/status"), command.Status.ToString())).ToArray();

        var changeSets = new JsonPatch(patchOperations);

        var eventId = await _serialNumberGenerator
            .GenerateAsync(cancellationToken)
            .ConfigureAwait(false);

        var currentTime = _systemClock.UtcNow.UtcDateTime;

        var agentInfo = await _agentRepository
            .GetAgentByIdAsync(command.AgentId, cancellationToken)
            .ConfigureAwait(false);

        await _agentRepository.CreateAgentEventAsync(
            eventId,
            agentInfo!.BrandId,
            command.AgentId,
            EventAction.Update,
            JsonSerializer.SerializeToDocument(changeSets),
            command.ModifyUserId,
            cancellationToken).ConfigureAwait(false);

        await _agentRepository.UpdateAgentSnapshotAndStatusAsync(
            command.AgentId,
            command.Status,
            eventId,
            currentTime,
            cancellationToken).ConfigureAwait(false);
    }
}