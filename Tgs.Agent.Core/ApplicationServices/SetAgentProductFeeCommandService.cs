using System.Text.Json;
using Json.Patch;
using Json.Pointer;
using Tgs.Agent.Core.Models;
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

        var fee = await repository.GetAgentProductFeeAsync(command.AgentId, cancellationToken);

        if (fee == null)
        {
            await repository
                .CreateAgentProductFeeAsync(
                    agentInfo!.BrandId, 
                    command.AgentId,
                    command.CurrentCardGamePercentage,
                    command.CurrentLiveDealerPercentage,
                    command.CurrentLotteryPercentage,
                    command.CurrentSlotPercentage,
                    command.CurrentSportsPercentage,
                    command.CurrentVirtualSportsPercentage,
                    command.NextCardGamePercentage,
                    command.NextLiveDealerPercentage,
                    command.NextLotteryPercentage,
                    command.NextSlotPercentage,
                    command.NextSportsPercentage,
                    command.NextVirtualSportsPercentage,
                    command.EditorUsername,
                    cancellationToken)
                .ConfigureAwait(false);

            await CreateLogAsync(agentInfo, fee, command, EventAction.Create);
        }
        else
        {
            await repository
                .UpdateAgentProductFeeAsync(command.AgentId,
                    command.CurrentCardGamePercentage,
                    command.CurrentLiveDealerPercentage,
                    command.CurrentLotteryPercentage,
                    command.CurrentSlotPercentage,
                    command.CurrentSportsPercentage,
                    command.CurrentVirtualSportsPercentage,
                    command.NextCardGamePercentage,
                    command.NextLiveDealerPercentage,
                    command.NextLotteryPercentage,
                    command.NextSlotPercentage,
                    command.NextSportsPercentage,
                    command.NextVirtualSportsPercentage,
                    command.EditorUsername,
                    cancellationToken)
                .ConfigureAwait(false);

            await CreateLogAsync(agentInfo, fee, command, EventAction.Update);
        }
    }

    private async Task CreateLogAsync(AgentInfo? agentInfo, AgentProductFeeInfo? feeInfo, SetAgentProductFeeCommand command, EventAction action)
    {
        var eventId = await serialNumberGenerator
            .GenerateAsync()
            .ConfigureAwait(false);

        var patchOperations = Enumerable.Empty<PatchOperation>();

        if (feeInfo?.CurrentCardGamePercentage != command.CurrentCardGamePercentage)
        {
            patchOperations = patchOperations.Append(PatchOperation.Replace(JsonPointer.Parse("/currentCardGamePercentage"),
                command.CurrentCardGamePercentage));
        }

        if (feeInfo?.CurrentLiveDealerPercentage != command.CurrentLiveDealerPercentage)
        {
            patchOperations = patchOperations.Append(PatchOperation.Replace(JsonPointer.Parse("/currentLiveDealerPercentage"),
                command.CurrentLiveDealerPercentage));
        }

        if (feeInfo?.CurrentLotteryPercentage != command.CurrentLotteryPercentage)
        {
            patchOperations = patchOperations.Append(PatchOperation.Replace(JsonPointer.Parse("/currentLotteryPercentage"),
                command.CurrentLotteryPercentage));
        }

        if (feeInfo?.CurrentSlotPercentage != command.CurrentSlotPercentage)
        {
            patchOperations = patchOperations.Append(PatchOperation.Replace(JsonPointer.Parse("/currentSlotPercentage"),
                command.CurrentSlotPercentage));
        }

        if (feeInfo?.CurrentSportsPercentage != command.CurrentSportsPercentage)
        {
            patchOperations = patchOperations.Append(PatchOperation.Replace(JsonPointer.Parse("/currentSportsPercentage"),
                command.CurrentSportsPercentage));
        }

        if (feeInfo?.CurrentVirtualSportsPercentage != command.CurrentVirtualSportsPercentage)
        {
            patchOperations = patchOperations.Append(PatchOperation.Replace(JsonPointer.Parse("/currentCardGamePercentage"),
                command.CurrentVirtualSportsPercentage));
        }

        if (feeInfo?.NextCardGamePercentage != command.NextCardGamePercentage)
        {
            patchOperations = patchOperations.Append(PatchOperation.Replace(JsonPointer.Parse("/nextCardGamePercentage"),
                command.NextCardGamePercentage));
        }

        if (feeInfo?.NextLiveDealerPercentage != command.NextLiveDealerPercentage)
        {
            patchOperations = patchOperations.Append(PatchOperation.Replace(JsonPointer.Parse("/nextLiveDealerPercentage"),
                command.NextLiveDealerPercentage));
        }

        if (feeInfo?.NextLotteryPercentage != command.NextLotteryPercentage)
        {
            patchOperations = patchOperations.Append(PatchOperation.Replace(JsonPointer.Parse("/nextLotteryPercentage"),
                command.NextLotteryPercentage));
        }

        if (feeInfo?.NextSlotPercentage != command.NextSlotPercentage)
        {
            patchOperations = patchOperations.Append(PatchOperation.Replace(JsonPointer.Parse("/nextSlotPercentage"),
                command.NextSlotPercentage));
        }

        if (feeInfo?.NextSportsPercentage != command.NextSportsPercentage)
        {
            patchOperations = patchOperations.Append(PatchOperation.Replace(JsonPointer.Parse("/nextSportsPercentage"),
                command.NextSportsPercentage));
        }

        if (feeInfo?.NextVirtualSportsPercentage != command.NextVirtualSportsPercentage)
        {
            patchOperations = patchOperations.Append(PatchOperation.Replace(JsonPointer.Parse("/nextCardGamePercentage"),
                command.NextVirtualSportsPercentage));
        }

        var changeSets = new JsonPatch(patchOperations);

        await repository.CreateAgentEventAsync(
            eventId,
            agentInfo!.BrandId,
            command.AgentId,
            action,
            JsonSerializer.SerializeToDocument(changeSets, AgentUtility.SerializerOptions),
            command.EditorUsername).ConfigureAwait(false);
    }
}