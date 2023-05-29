using Microsoft.AspNetCore.Mvc;
using Tgs.Agent.Core.ApplicationServices;
using Tgs.Agent.Core.Models;
using Tgs.Agent.Service.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.Agent.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AgentController : ControllerBase
{
    [HttpGet("allChildAgents/{agentId:Guid}", Name = "GetChildAgentInfos")]
    public async IAsyncEnumerable<ChildAgentInfoViewModel> GetChildAgentInfosAsync(
        [FromServices] IQueryService<GetChildAgentInfosQuery, ChildAgentInfoModel> queryService,
        Guid agentId)
    {
        var infos = await queryService
            .QueryAsync(new(agentId), HttpContext.RequestAborted)
            .ToArrayAsync();

        foreach (var info in infos)
        {
            yield return new ChildAgentInfoViewModel()
            {
                AgentId = info.AgentId,
                AgentUsername = info.AgentUsername,
                ParentAgentId = info.ParentAgentId,
            };
        }
    }
}