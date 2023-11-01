using System.Text.Json;
using Json.Patch;
using Json.Pointer;
using Tgs.Core.Infrastructures.Security;
using Tgs.SerialNumberGenerator;
using Tgs.ServiceUtility;

namespace Tgs.Agent.Core.ApplicationServices;

internal class SetAgentPasswordCommandService : ICommandService<SetAgentPasswordCommand>
{
    private readonly IAgentRepository _repository;
    private readonly IPasswordHandler _passwordHandler;
    private readonly ISerialNumberGenerator _serialNumberGenerator;

    public SetAgentPasswordCommandService(
        IAgentRepository repository,
        IPasswordHandler passwordHandler,
        ISerialNumberGenerator serialNumberGenerator)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _passwordHandler = passwordHandler ?? throw new ArgumentNullException(nameof(passwordHandler));
        _serialNumberGenerator = serialNumberGenerator ?? throw new ArgumentNullException(nameof(serialNumberGenerator));
    }

    public async ValueTask ExecuteAsync(SetAgentPasswordCommand command, CancellationToken cancellationToken = default)
    {
        var (salt, hashedPassword) = _passwordHandler.GenerateCredential(command.Password);

        await _repository.SetPasswordAsync(
            command.AgentId,
            hashedPassword,
            salt,
            cancellationToken).ConfigureAwait(false);

        var patchOperations = Array.Empty<PatchOperation>();
        patchOperations = patchOperations
            .Append(PatchOperation.Replace(JsonPointer.Parse($"/password"), ""))
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