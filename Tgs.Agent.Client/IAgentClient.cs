using Tgs.Agent.Client.Models;

namespace Tgs.Agent.Client;

public interface IAgentClient
{
    ValueTask<AgentInfoModel?> GetAgentInfoAsync(GetAgentInfoRequest request, CancellationToken cancellationToken = default);

    ValueTask<AgentInfoModel?> GetAgentInfoByUsernameAsync(GetAgentInfoByUsernameRequest request, CancellationToken cancellationToken = default);

    IAsyncEnumerable<ChildAgentInfoModel> GetChildAgentInfosAsync(GetChildAgentInfosRequest request, CancellationToken cancellationToken = default);

    ValueTask SetAgentInfoAsync(SetAgentInfoRequest request, CancellationToken cancellationToken = default);

    ValueTask SetAgentPasswordAsync(SetAgentPasswordRequest request, CancellationToken cancellationToken = default);
}