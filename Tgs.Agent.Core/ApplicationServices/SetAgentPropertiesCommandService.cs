using System.Text.Json;
using Json.Patch;
using Json.Pointer;
using Tgs.SerialNumberGenerator;
using Tgs.ServiceUtility;

namespace Tgs.Agent.Core.ApplicationServices;

internal class SetAgentPropertiesCommandService(
    IAgentRepository agentRepository,
    IDataCommandBuilder dataCommandBuilder,
    ISerialNumberGenerator serialNumberGenerator)
    : ICommandService<SetAgentPropertiesCommand>
{
    private readonly IAgentRepository _agentRepository = agentRepository ?? throw new ArgumentNullException(nameof(agentRepository));
    private readonly IDataCommandBuilder _dataCommandBuilder = dataCommandBuilder ?? throw new ArgumentNullException(nameof(dataCommandBuilder));
    private readonly ISerialNumberGenerator _serialNumberGenerator = serialNumberGenerator ?? throw new ArgumentNullException(nameof(serialNumberGenerator));

    public async ValueTask ExecuteAsync(SetAgentPropertiesCommand command, CancellationToken cancellationToken = default)
    {
        var agent = await _agentRepository
            .GetAgentByIdAsync(command.AgentId, cancellationToken)
            .ConfigureAwait(false);

        if (agent == null)
            return;

        var availablePropertyNames = new HashSet<string>(
            Enum.GetNames<AgentPropertyName>(),
            StringComparer.OrdinalIgnoreCase);
        var dataCommands = new List<IDataCommand>();

        var patchOperations = Array.Empty<PatchOperation>();

        foreach (var (key, value) in command.Properties
            .Select(kv => (
                KV: kv,
                RegularName: availablePropertyNames.TryGetValue(kv.Key, out var regularName)
                    ? regularName
                    : default))
            .Where(g => g.RegularName is not null)
            .ToDictionary(g => g.RegularName!, g => g.KV.Value))
        {
            var source = $"/{key}";

            if (value == null)
            {
                dataCommands.Add(_dataCommandBuilder.CreateDeleteCommand(key));
                patchOperations = patchOperations.Append(PatchOperation.Remove(JsonPointer.Parse(source))).ToArray();
            }
            else
            {
                dataCommands.Add(_dataCommandBuilder.CreatePutCommand(key, value));
                patchOperations = patchOperations.Append(PatchOperation.Replace(JsonPointer.Parse(source), value)).ToArray();
            }
        }

        var changeSets = new JsonPatch(patchOperations);
        var eventId = await _serialNumberGenerator
            .GenerateAsync(cancellationToken)
            .ConfigureAwait(false);

        var currentTime = DateTimeOffset.UtcNow.UtcDateTime;

        await _agentRepository.CreateAgentEventAsync(
            eventId,
            agent.BrandId,
            agent.Id,
            EventAction.Update,
            JsonSerializer.SerializeToDocument(changeSets, AgentUtility.SerializerOptions),
            command.ModifyUserId,
            cancellationToken).ConfigureAwait(false);

        await _agentRepository.UpdateAgentSnapshotAndPropertiesAsync(
            agent.Id,
            agent.BrandId,
            eventId,
            dataCommands,
            currentTime,
            cancellationToken).ConfigureAwait(false);
    }
}