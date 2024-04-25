using Tgs.Agent.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.Agent.Core.ApplicationServices;

internal class GetAgentInfoQueryService(IAgentRepository repository) : IGetService<GetAgentInfoQuery, AgentInfo?>
{
    private readonly IAgentRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public ValueTask<AgentInfo?> GetAsync(GetAgentInfoQuery query, CancellationToken cancellationToken = default)
        => _repository.GetAgentByIdAsync(query.AgentId, cancellationToken);
}