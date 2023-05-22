using Microsoft.AspNetCore.Mvc;
using Tgs.Agent.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.Agent.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AgentController : ControllerBase
{
    //[HttpGet("{agentId}")]
    //public async ValueTask<AgentGroupCountInfo> GetAgents([FromServices] IGetService<GetAgentGroupCountInfoRequest, GetAgentGroupCountInfoModel> queryService,
    //    Guid agentId)
    //{
    //    var response = await queryService.GetAsync(new GetAgentGroupCountInfoRequest(agentId));

    //    return new AgentGroupCountInfo
    //    {
    //        DirectAgentCount = response.DirectAgentCount,
    //        DirectPlayerCount = response.DirectPlayerCount,
    //        IndirectAgentCount = response.IndirectAgentCount,
    //        IndirectPlayerCount = response.IndirectPlayerCount,
    //    };
    //}

    [HttpGet("allChildAgents/{agentId:Guid}", Name = "GetChildAgentInfos")]
    public IAsyncEnumerable<ChildAgentInfoModel> GetChildAgentInfos(
        [FromServices] IQueryService<GetChildAgentInfosRequest, ChildAgentInfoModel> queryService,
        Guid agentId)
        => queryService.QueryAsync(new(agentId), HttpContext.RequestAborted);
}