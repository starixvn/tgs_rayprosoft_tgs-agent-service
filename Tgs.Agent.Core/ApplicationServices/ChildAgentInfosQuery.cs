using Tgs.Agent.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.Agent.Core.ApplicationServices;

internal class ChildAgentInfosQuery : IQueryService<GetChildAgentInfosRequest, ChildAgentInfoModel>
{
    private readonly IAgentRepository _repository;

    public ChildAgentInfosQuery(IAgentRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public IAsyncEnumerable<ChildAgentInfoModel> QueryAsync(GetChildAgentInfosRequest query, CancellationToken cancellationToken = default)
        => _repository.GetChildAgentInfosAsync(query.AgentId, cancellationToken);
}