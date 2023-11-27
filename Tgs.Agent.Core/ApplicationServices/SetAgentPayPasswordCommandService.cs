using Json.Patch;
using Json.Pointer;
using System.Text.Json;
using Tgs.Core.Infrastructures.Security;
using Tgs.SerialNumberGenerator;
using Tgs.ServiceUtility;

namespace Tgs.Agent.Core.ApplicationServices;

internal class SetAgentPayPasswordCommandService : ICommandService<SetAgentPayPasswordCommand>
{
    private readonly IAgentRepository _repository;
    private readonly IPasswordHandler _passwordHandler;
    private readonly ISerialNumberGenerator _serialNumberGenerator;

    public SetAgentPayPasswordCommandService(
        IAgentRepository agentRepository,
        IPasswordHandler passwordHandler,
        ISerialNumberGenerator serialNumberGenerator)
    {
        _repository = agentRepository ?? throw new ArgumentNullException(nameof(agentRepository));
        _passwordHandler = passwordHandler ?? throw new ArgumentNullException(nameof(passwordHandler));
        _serialNumberGenerator = serialNumberGenerator ?? throw new ArgumentNullException(nameof(serialNumberGenerator));
    }

    public async ValueTask ExecuteAsync(SetAgentPayPasswordCommand command, CancellationToken cancellationToken = default)
    {
        var (paySalt, hashedPayPassword) = _passwordHandler.GenerateCredential(command.PayPassword);

        await _repository.SetPayPasswordAsync(
            command.AgentId,
            hashedPayPassword,
            paySalt,
            cancellationToken).ConfigureAwait(false);

        var patchOperations = Array.Empty<PatchOperation>();
        patchOperations = patchOperations
            .Append(PatchOperation.Replace(JsonPointer.Parse($"/withdrawPassword"), ""))
            .ToArray();

        var changeSets = new JsonPatch(patchOperations);
        var eventId = await _serialNumberGenerator
            .GenerateAsync(cancellationToken)
            .ConfigureAwait(false);
        var currentTime = DateTimeOffset.UtcNow.UtcDateTime;

        var agentInfo = await _repository
            .GetAgentByIdAsync(command.AgentId, cancellationToken)
            .ConfigureAwait(false);

        await _repository.CreateAgentEventAsync(
            eventId,
            agentInfo!.BrandId,
            command.AgentId,
            EventAction.Update,
            JsonSerializer.SerializeToDocument(changeSets),
            command.ModifyUserId,
            cancellationToken).ConfigureAwait(false);

        await _repository.UpdateAgentSnapshotAsync(
            command.AgentId,
            eventId,
            currentTime, cancellationToken).ConfigureAwait(false);
    }
}