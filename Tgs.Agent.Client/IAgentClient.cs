using Tgs.Agent.Client.Models;

namespace Tgs.Agent.Client;

public interface IAgentClient
{
    IAsyncEnumerable<ChildAgentInfoModel> GetChildAgentInfosAsync(GetChildAgentInfosRequest request, CancellationToken cancellationToken = default);

    ValueTask ResetSubAgentPasswordAsync(ResetSubAgentPasswordRequest request, CancellationToken cancellationToken = default);

    ValueTask SetSubAgentInfoAsync(SetSubAgentInfoRequest request, CancellationToken cancellationToken = default);
}