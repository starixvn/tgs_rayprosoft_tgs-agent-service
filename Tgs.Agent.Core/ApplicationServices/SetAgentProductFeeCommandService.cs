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
            PatchOperation.Replace(JsonPointer.Parse("/cardGamePercentage"),
                    command.CardGamePercentage),
            PatchOperation.Replace(JsonPointer.Parse("/liveDealerPercentage"),
                    command.LiveDealerPercentage),
            PatchOperation.Replace(JsonPointer.Parse("/lotteryPercentage"),
                    command.LotteryPercentage),
            PatchOperation.Replace(JsonPointer.Parse("/slotPercentage"),
                    command.SlotPercentage),
            PatchOperation.Replace(JsonPointer.Parse("/sportsPercentage"),
                    command.SportsPercentage),
            PatchOperation.Replace(JsonPointer.Parse("/virtualSportsPercentage"),
                    command.VirtualSportsPercentage),
        ];

        var changeSets = new JsonPatch(patchOperations);

        var eventId = await serialNumberGenerator
            .GenerateAsync(cancellationToken)
        .ConfigureAwait(false);

        var fee = await repository.GetAgentProductFeeAsync(command.AgentId, cancellationToken);

        if (fee == null)
        {
            await repository
                .CreateAgentProductFeeAsync(command.AgentId,
                    command.CardGamePercentage,
                    command.LiveDealerPercentage,
                    command.LotteryPercentage,
                    command.SlotPercentage,
                    command.SportsPercentage,
                    command.VirtualSportsPercentage,
                    command.EditorUsername,
                    cancellationToken)
                .ConfigureAwait(false);
        }
        else
        {
            await repository
                .UpdateAgentProductFeeAsync(command.AgentId,
                    command.CardGamePercentage,
                    command.LiveDealerPercentage,
                    command.LotteryPercentage,
                    command.SlotPercentage,
                    command.SportsPercentage,
                    command.VirtualSportsPercentage,
                    command.EditorUsername,
                    cancellationToken)
                .ConfigureAwait(false);
        }

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