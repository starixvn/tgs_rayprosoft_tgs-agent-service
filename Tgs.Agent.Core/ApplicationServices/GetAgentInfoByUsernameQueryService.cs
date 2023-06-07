using Tgs.Agent.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.Agent.Core.ApplicationServices;

internal class GetAgentInfoByUsernameQueryService : IGetService<GetAgentInfoByUsernameQuery, AgentInfo?>
{
    private readonly IAgentRepository _repository;

    public GetAgentInfoByUsernameQueryService(IAgentRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public ValueTask<AgentInfo?> GetAsync(GetAgentInfoByUsernameQuery query, CancellationToken cancellationToken = default)
        => _repository.GetAgentByUsernameAsync(query.BrandId, query.Username, cancellationToken);
}