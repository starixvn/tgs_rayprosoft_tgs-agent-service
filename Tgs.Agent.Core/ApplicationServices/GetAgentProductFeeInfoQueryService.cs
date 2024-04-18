using Tgs.Agent.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.Agent.Core.ApplicationServices;

internal class GetAgentProductFeeInfoQueryService(IAgentRepository repository)
    : IGetService<GetAgentProductFeeInfoQuery, AgentProductFeeInfo?>
{
    private readonly IAgentRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public ValueTask<AgentProductFeeInfo?> GetAsync(GetAgentProductFeeInfoQuery query, CancellationToken cancellationToken = default)
        => _repository.GetAgentProductFeeAsync(query.AgentId, cancellationToken);
}