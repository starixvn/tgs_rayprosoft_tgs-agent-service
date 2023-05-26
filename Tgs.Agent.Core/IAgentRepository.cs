using Tgs.Agent.Core.Models;

namespace Tgs.Agent.Core
{
    public interface IAgentRepository
    {
        IAsyncEnumerable<ChildAgentInfoModel> GetChildAgentInfosAsync(Guid agentId);
    }
}
