using Tgs.Agent.Client.Models;

namespace Tgs.Agent.Client;

public interface IAgentClient
{
    IAsyncEnumerable<ChildAgentInfoModel> GetChildAgentInfosAsync(GetChildAgentInfosRequest request);
}