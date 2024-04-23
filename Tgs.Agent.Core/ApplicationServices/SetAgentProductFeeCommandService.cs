using System.Text.Json;
using Json.Patch;
using Json.Pointer;
using Tgs.SerialNumberGenerator;
using Tgs.ServiceUtility;

namespace Tgs.Agent.Core.ApplicationServices;

internal class SetAgentProductFeeCommandService(
    IAgentRepository repository,
    ISerialNumberGenerator serialNumberGenerator)
    : ICommandService<SetAgentProductFeeCommand>
{
    public async ValueTask ExecuteAsync(SetAgentProductFeeCommand command, CancellationToken cancellationToken = default)
    {
        var agentInfo = await repository
            .GetAgentByIdAsync(command.AgentId, cancellationToken)
            .ConfigureAwait(false);

        var patchOperations = Array.Empty<PatchOperation>();

        patchOperations =
        [
            .. patchOperations,
            PatchOperation.Replace(JsonPointer.Parse("/cardGameRebatePercentage"),
                    command.CardGameRebatePercentage),
            PatchOperation.Replace(JsonPointer.Parse("/liveDealerRebatePercentage"),
                    command.LiveDealerRebatePercentage),
            PatchOperation.Replace(JsonPointer.Parse("/lotteryRebatePercentage"),
                    command.LotteryRebatePercentage),
            PatchOperation.Replace(JsonPointer.Parse("/slotRebatePercentage"),
                    command.SlotRebatePercentage),
            PatchOperation.Replace(JsonPointer.Parse("/sportsRebatePercentage"),
                    command.SportsRebatePercentage),
            PatchOperation.Replace(JsonPointer.Parse("/virtualSportsRebatePercentage"),
                    command.VirtualSportsRebatePercentage),
        ];

        var changeSets = new JsonPatch(patchOperations);

        var eventId = await serialNumberGenerator
            .GenerateAsync(cancellationToken)
            .ConfigureAwait(false);

        await repository
            .UpdateAgentProductFeeAsync(command.AgentId,
                command.CardGameRebatePercentage,
                command.LiveDealerRebatePercentage,
                command.LotteryRebatePercentage,
                command.SlotRebatePercentage,
                command.SportsRebatePercentage,
                command.VirtualSportsRebatePercentage,
                command.EditorUsername,
                eventId,
                cancellationToken)
            .ConfigureAwait(false);

        await repository.CreateAgentEventAsync(
            eventId,
            agentInfo!.BrandId,
            command.AgentId,
            EventAction.Update,
            JsonSerializer.SerializeToDocument(changeSets, AgentUtility.SerializerOptions),
            command.EditorUsername,
            cancellationToken).ConfigureAwait(false);
    }
}