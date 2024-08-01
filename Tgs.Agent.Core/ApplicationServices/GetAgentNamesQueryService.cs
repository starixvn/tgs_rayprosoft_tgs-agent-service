using Tgs.Agent.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.Agent.Core.ApplicationServices;

internal class GetAgentNamesQueryService(IAgentRepository repository)
    : IQueryService<GetAgentNamesQuery, AgentNameInfo>
{
    private readonly IAgentRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public IAsyncEnumerable<AgentNameInfo> QueryAsync(GetAgentNamesQuery query, CancellationToken cancellationToken = default)
        => _repository.GetAgentNamesAsync(query.BrandId);
}