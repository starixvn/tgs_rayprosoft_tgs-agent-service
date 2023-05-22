using Tgs.Agent.Core.Models;
using Tgs.Agent.Core.RepositoryModels;
using Tgs.ServiceUtility;

namespace Tgs.Agent.Core.Application;

internal class ChildAgentInfosQuery : IQueryService<GetChildAgentInfosRequest,
    ChildAgentInfoModel>
{
    private readonly tgsContext _context;

    public ChildAgentInfosQuery(tgsContext context)
    {
        _context = context;
    }

    public IAsyncEnumerable<ChildAgentInfoModel> QueryAsync(GetChildAgentInfosRequest query,
        CancellationToken cancellationToken)
        => _context.Agents
            .Where(x => x.Id != query.AgentId)
            .Where(x => x.Path.MatchesLQuery($"*.{query.AgentId:N}.*"))
            .Select(x => new ChildAgentInfoModel
            {
                AgentId = x.Id,
                ParentAgentId = x.ParentAgentId!.Value
            })
            .ToAsyncEnumerable();
}