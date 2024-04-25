using Tgs.ServiceUtility;

namespace Tgs.Agent.Core.ApplicationServices;

internal class GetAgentAuditLogQueryService(IAgentRepository repository)
    : IQueryService<GetAgentAuditLogQuery, AgentAuditLogInfo>, IGetService<GetAgentAuditLogQuery, int>
{
    private readonly IAgentRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public ValueTask<int> GetAsync(GetAgentAuditLogQuery query, CancellationToken cancellationToken = default)
        => _repository.CountAgentAuditLogsAsync(
            new AgenAuditLogQueryConditions(
                query.BrandId,
                query.AgentId,
                query.From,
                query.To),
            cancellationToken);

    public IAsyncEnumerable<AgentAuditLogInfo> QueryAsync(GetAgentAuditLogQuery query, CancellationToken cancellationToken = default)
        => _repository.QueryAgentAuditLogsAsync(
            new AgenAuditLogQueryConditions(
                query.BrandId,
                query.AgentId,
                query.From,
                query.To),
            query.Paging,
            cancellationToken);
}