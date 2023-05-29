//using Microsoft.EntityFrameworkCore;
//using Tgs.Agent.Core.Models;
//using Tgs.Agent.Core.RepositoryModels;
//using Tgs.PlayerService.Client;
//using Tgs.ServiceUtility;

//namespace Tgs.Agent.Core.ApplicationServices;

//internal class AgentGroupCountInfoQuery : IGetService<GetAgentGroupCountInfoRequest,
//    GetAgentGroupCountInfoModel>
//{
//    private readonly tgsContext _context;
//    private readonly IPlayerServiceClient _playerServiceClient;

//    public AgentGroupCountInfoQuery(tgsContext context, IPlayerServiceClient playerServiceClient)
//    {
//        _context = context;
//        _playerServiceClient = playerServiceClient;
//    }

//    public async ValueTask<GetAgentGroupCountInfoModel> GetAsync(GetAgentGroupCountInfoRequest query,
//        CancellationToken cancellationToken = new())
//    {
//        var allChildAgents = await _context.Agents
//            .Where(x => x.Id != query.AgentId)
//            .Where(x => x.Path.MatchesLQuery($"*.{query.AgentId:N}.*"))
//            .Select(x => new
//            {
//                AgentId = x.Id,
//                x.ParentAgentId,
//                x.Path
//            })
//            .ToArrayAsync(cancellationToken);

//        var directAgents = allChildAgents.Where(x => x.ParentAgentId == query.AgentId);

//        var indirectAgents = allChildAgents.Where(x => !directAgents.Contains(x)).ToArray();

//        var agentPlayerCounts = _playerServiceClient.GetPlayerCountInfosByBrandIdAsync(query.BrandId, cancellationToken);

//        var directPlayerCountInfo = await agentPlayerCounts.FirstOrDefaultAsync(x => x.AgentId == query.AgentId, cancellationToken);

//        var allChildAgentIds = allChildAgents.Select(x => x.AgentId);

//        var indirectPlayerCount =
//            await agentPlayerCounts.Where(x => x.AgentId != null && allChildAgentIds.Contains(x.AgentId.Value))
//                .SumAsync(x => x.PlayerCount, cancellationToken);

//        return new()
//        {
//            DirectAgentCount = directAgents.Count(),
//            DirectPlayerCount = directPlayerCountInfo?.PlayerCount ?? 0,
//            IndirectAgentCount = indirectAgents.Count(),
//            IndirectPlayerCount = indirectPlayerCount
//        };
//    }
//}

