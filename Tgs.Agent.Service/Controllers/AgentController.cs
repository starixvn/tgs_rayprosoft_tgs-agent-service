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
    [HttpGet("info/{agentId}", Name = "GetAgentInfo")]
    public async ValueTask<AgentInfoViewModel?> GetAgentInfoAsync(
        [FromServices] IGetService<GetAgentInfoQuery, AgentInfo?> getService,
        Guid agentId)
    {
        var info = await getService
            .GetAsync(new GetAgentInfoQuery(agentId), HttpContext.RequestAborted)
            .ConfigureAwait(false);

        if (info is null)
            return null;

        return new AgentInfoViewModel(
            info.Id,
            info.Username,
            info.RealName,
            info.Phone,
            info.Email,
            info.QQ,
            info.Wechat,
            info.InvitationCode,
            info.ParentAgentId,
            info.RegisteredOn,
            info.Status,
            info.Type,
            info.AgentLevelType,
            info.DividendPercentage,
            info.PlayerNumber,
            info.AccumulatedProfitSharing);
    }

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

    [HttpPatch("setInfo", Name = "SetAgentInfo")]
    public ValueTask SetAgentInfoAsync(
       [FromServices] ICommandService<SetAgentInfoCommand> commandService,
       [FromBody] SetAgentInfoViewModel request)
        => commandService.ExecuteAsync(
        new SetAgentInfoCommand(
                    request.AgentId,
                    request.RealName,
                    request.Email,
                    request.Phone),
            HttpContext.RequestAborted);

    [HttpPatch("setPassword", Name = "SetPassword")]
    public ValueTask SetPasswordAsync(
        [FromServices] ICommandService<SetAgentPasswordCommand> commandService,
        [FromBody] SetAgentPasswordViewModel request)
        => commandService.ExecuteAsync(
                new(
                    request.AgentId,
                    request.Password),
            HttpContext.RequestAborted);
}