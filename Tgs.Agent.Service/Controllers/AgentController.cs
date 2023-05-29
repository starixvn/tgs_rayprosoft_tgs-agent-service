using Microsoft.AspNetCore.Mvc;
using Tgs.Agent.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.Agent.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AgentController : ControllerBase
{
    [HttpGet("allChildAgents/{agentId:Guid}", Name = "GetChildAgentInfos")]
    public IAsyncEnumerable<ChildAgentInfoModel> GetChildAgentInfosAsync(
        [FromServices] IQueryService<GetChildAgentInfosRequest, ChildAgentInfoModel> queryService,
        Guid agentId)
        => queryService.QueryAsync(new(agentId), HttpContext.RequestAborted);
}