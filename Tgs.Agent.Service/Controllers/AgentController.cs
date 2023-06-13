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
    [HttpGet("{agentId}", Name = "GetAgentInfo")]
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

    [HttpGet("info/{brandId}/{username}", Name = "GetAgentInfoByUsername")]
    public async ValueTask<AgentInfoViewModel?> GetAgentInfoByUsernameAsync(
        [FromServices] IGetService<GetAgentInfoByUsernameQuery, AgentInfo?> getService,
        Guid brandId, string username)
    {
        var info = await getService
            .GetAsync(new GetAgentInfoByUsernameQuery(brandId, username), HttpContext.RequestAborted)
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
                DividendPercentage = info.DividendPercentage,
            };
        }
    }

    [HttpPatch("{agentId}:info", Name = "SetAgentInfo")]
    public ValueTask SetAgentInfoAsync(
       [FromServices] ICommandService<SetAgentInfoCommand> commandService,
       [FromBody] SetAgentInfoViewModel request,
       Guid agentId)
        => commandService.ExecuteAsync(
        new SetAgentInfoCommand(
                    agentId,
                    request.RealName,
                    request.Email,
                    request.Phone),
            HttpContext.RequestAborted);

    [HttpPatch("{agentId}:dividend", Name = "SetAgentDividendPercentage")]
    public ValueTask SetAgentDividendPercentageAsync(
       [FromServices] ICommandService<SetAgentDividendPercentageCommand> commandService,
       [FromBody] SetAgentDividendPercentageViewModel request,
       Guid agentId)
        => commandService.ExecuteAsync(
        new SetAgentDividendPercentageCommand(
                    agentId,
                    request.DividendPercentage),
            HttpContext.RequestAborted);

    [HttpPatch("{agentId}:password", Name = "SetPassword")]
    public ValueTask SetPasswordAsync(
        [FromServices] ICommandService<SetAgentPasswordCommand> commandService,
        [FromBody] SetAgentPasswordViewModel request,
        Guid agentId)
        => commandService.ExecuteAsync(
                new(
                    agentId,
                    request.Password),
            HttpContext.RequestAborted);
}