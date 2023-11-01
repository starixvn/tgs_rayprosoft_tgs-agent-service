using Tgs.ServiceUtility;

namespace Tgs.Agent.Core.ApplicationServices;

internal class GetAgentHistoriesQueryService
    : IQueryService<GetAgentHistoriesQuery, AgentHistoryInfo>, IGetService<GetAgentHistoriesQuery, int>
{
    private readonly IAgentRepository _repository;

    public GetAgentHistoriesQueryService(IAgentRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public ValueTask<int> GetAsync(GetAgentHistoriesQuery query, CancellationToken cancellationToken = default)
        => _repository.CountAgentEventAsync(new AgentEventQueryConditions(query.AgentId, query.From, query.To), cancellationToken);

    public IAsyncEnumerable<AgentHistoryInfo> QueryAsync(GetAgentHistoriesQuery query, CancellationToken cancellationToken = default)
        => _repository.QueryAgentEventAsync(new AgentEventQueryConditions(
            query.AgentId,
            query.From,
            query.To),
            query.Skip,
            cancellationToken);
}