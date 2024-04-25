using Tgs.Agent.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.Agent.Core.ApplicationServices;

internal class GetChildAgentInfosQueryService(IAgentRepository repository)
    : IQueryService<GetChildAgentInfosQuery, ChildAgentInfoModel>
{
    private readonly IAgentRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public IAsyncEnumerable<ChildAgentInfoModel> QueryAsync(
        GetChildAgentInfosQuery query,
        CancellationToken cancellationToken = default)
        => _repository.GetChildAgentInfosAsync(query.AgentId, cancellationToken);
}