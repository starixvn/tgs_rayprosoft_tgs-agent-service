using Tgs.Agent.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.Agent.Core.Application;

internal class GetChildAgentInfosQuery : IQueryService<GetChildAgentInfosRequest,
    ChildAgentInfoModel>
{
    private readonly IAgentRepository _agentRepository;

    public GetChildAgentInfosQuery(IAgentRepository agentRepository)
    {
        _agentRepository = agentRepository;
    }

    public IAsyncEnumerable<ChildAgentInfoModel> QueryAsync(GetChildAgentInfosRequest query,
        CancellationToken cancellationToken)
        => _agentRepository.GetChildAgentInfosAsync(query.AgentId);
}