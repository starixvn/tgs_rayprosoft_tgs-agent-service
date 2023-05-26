using Tgs.Agent.Core;
using Tgs.Agent.Core.Models;
using Tgs.Agent.Repositories.RepositoryModels;

namespace TGS.Agent.Repositories;

internal class AgentRepository : IAgentRepository
{
    private readonly tgsContext _context;

    public AgentRepository(tgsContext context)
        => _context = context ?? throw new ArgumentNullException(nameof(context));

    public IAsyncEnumerable<ChildAgentInfoModel> GetChildAgentInfosAsync(Guid agentId)
        => _context.Agents
            .Where(x => x.Id != agentId)
            .Where(x => x.Path.MatchesLQuery($"*.{agentId:N}.*"))
            .Select(x => new ChildAgentInfoModel
            {
                AgentId = x.Id,
                AgentUsername = x.Username,
                ParentAgentId = x.ParentAgentId!.Value
            })
            .ToAsyncEnumerable();
}
