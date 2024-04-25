using Tgs.Agent.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.Agent.Core.ApplicationServices;

internal class GetAgentInfoByUsernameQueryService(IAgentRepository repository)
    : IGetService<GetAgentInfoByUsernameQuery, AgentInfo?>
{
    private readonly IAgentRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public ValueTask<AgentInfo?> GetAsync(GetAgentInfoByUsernameQuery query, CancellationToken cancellationToken = default)
        => _repository.GetAgentByUsernameAsync(query.BrandId, query.Username, cancellationToken);
}