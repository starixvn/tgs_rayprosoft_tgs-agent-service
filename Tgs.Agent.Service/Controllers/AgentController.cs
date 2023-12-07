using System.Collections.Immutable;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using Tgs.Agent.Core;
using Tgs.Agent.Core.ApplicationServices;
using Tgs.Agent.Core.Models;
using Tgs.Agent.Service.ViewModels;
using Tgs.ServiceUtility;
using Tgs.WebApi.Toolkits;

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
            info.AccumulatedProfitSharing,
            info.BrandId);
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
            info.AccumulatedProfitSharing,
            info.BrandId);
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
                    request.RealName),
            HttpContext.RequestAborted);

    [HttpPatch("{agentId}:contact", Name = "SetAgentContact")]
    public ValueTask SetAgentContactAsync(
       [FromServices] ICommandService<SetAgentContactCommand> commandService,
       [FromBody] SetAgentContactViewModel request,
       Guid agentId)
        => commandService.ExecuteAsync(
        new SetAgentContactCommand(
                    agentId,
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
        [FromHeader(Name = "ModifyUserId")] string? modifyUserId,
        [FromBody] SetAgentPasswordViewModel request,
        Guid agentId)
        => commandService.ExecuteAsync(
                new(
                    agentId,
                    request.Password,
                    modifyUserId),
            HttpContext.RequestAborted);

    [HttpPatch("{agentId}:pay-password", Name = "SetPayPassword")]
    public ValueTask SetPayPasswordAsync(
        [FromServices] ICommandService<SetAgentPayPasswordCommand> commandService,
        [FromBody] SetAgentPayPasswordViewModel request,
        Guid agentId)
        => commandService.ExecuteAsync(
            new SetAgentPayPasswordCommand(
                agentId,
                request.PayPassword),
            HttpContext.RequestAborted);

    [HttpPost("prop/{agentId}", Name = "SetAgentProperties")]
    [SuccessStatusCode(200, "Set agent properties success")]
    [FailStatusCode(400, "Input properties is empty")]
    public ValueTask SetPlayerPropertiesAsync(
        [FromServices] ICommandService<SetAgentPropertiesCommand> commandService,
        [FromHeader(Name = "ModifyUserId")] string? modifyUserId,
        Guid agentId,
        [FromBody] IImmutableDictionary<string, string?> properties)
    {
        if (properties.Count == 0)
            return new ValueTask(this.FailAsync(400, "Input properties is empty"));

        var command = new SetAgentPropertiesCommand(
            agentId,
            properties,
            modifyUserId);

        return commandService
            .ExecuteAsync(command, HttpContext.RequestAborted);
    }

    [HttpPut("status/{agentId}", Name = "SetAgentStatus")]
    [SuccessStatusCode(200, "Status set")]
    public ValueTask SetPlayerStatusAsync(
        [FromServices] ICommandService<SetAgentStatusCommand> setService,
        [FromHeader(Name = "ModifyUserId")] string? modifyUserId,
        Guid agentId,
        [FromBody] AgentStatus status)
    {
        var command = new SetAgentStatusCommand(agentId, status, modifyUserId);

        return setService.ExecuteAsync(command, HttpContext.RequestAborted);
    }

    [HttpGet("histories/{agentId}", Name = "GetAgentHistories")]
    [Description("查詢代理操作紀錄")]
    public IAsyncEnumerable<AgentHistoryInfoViewModel> GetAgentHistoriesAsync(
        [FromServices] IQueryService<GetAgentHistoriesQuery, AgentHistoryInfo> queryService,
        Guid agentId,
        [FromQuery] AgentHistoryQueryViewModel request,
        [FromQuery(Name = "skip")] int skip = 0)
    {
        var query = new GetAgentHistoriesQuery(agentId, request.From, request.To, skip);

        return queryService.QueryAsync(query, HttpContext.RequestAborted)
            .Select(item => new AgentHistoryInfoViewModel(
                item.EventId,
                item.Action,
                item.Modifier,
                item.CreatedOn));
    }

    [HttpHead("histories/{agentId}", Name = "CountAgentHistories")]
    [Description("查詢代理操作紀錄總筆數")]
    public async Task<NoContentResult> CountAgentHistoriesAsync(
        [FromServices] IGetService<GetAgentHistoriesQuery, int> countService,
        Guid agentId,
        [FromQuery] AgentHistoryQueryViewModel request,
        [FromQuery(Name = "skip")] int skip = 0)
    {
        var query = new GetAgentHistoriesQuery(agentId, request.From, request.To, skip);

        var count = await countService
            .GetAsync(query, HttpContext.RequestAborted)
            .ConfigureAwait(false);

        HttpContext.Response.Headers.Add("Tgs-Return", count.ToString());

        return NoContent();
    }

    [HttpGet("history/{eventId}", Name = "GetAgentHistory")]
    [SuccessStatusCode(200, "Agent event info found")]
    [FailStatusCode(404, "Agent event info not found")]
    public async ValueTask<AgentEventInfoViewModel?> GetAgentHistoryAsync(
        [FromServices] IGetService<GetAgentEventQuery, EventInfo> getService,
        Guid eventId)
    {
        var result = await getService.GetAsync(new GetAgentEventQuery(eventId)).ConfigureAwait(false);

        return result is null
            ? await this.FailAsync<AgentEventInfoViewModel, object?>(404, default)
            : new AgentEventInfoViewModel
            {
                Action = result.Action,
                ChangeSets = result.ChangeSets,
                Modifier = result.Modifier,
                CreatedOn = result.CreatedOn,
            };
    }

    [HttpPatch("{agentId}:sub-agents/path/{newParentAgentId}", Name = "SetSubAgentsPath")]
    public ValueTask SetSubAgentsPathAsync(
        [FromServices] ICommandService<SetSubAgentsPathCommand> commandService,
        Guid agentId,
        Guid newParentAgentId)
    {
        var command = new SetSubAgentsPathCommand(agentId, newParentAgentId);

        return commandService.ExecuteAsync(command);
    }

    [HttpPost("", Name = "CreateAgent")]
    [Description("Create an agent")]
    public ValueTask<Guid> CreateAgentAsync([FromServices] IGetService<GetAndAddAgentQuery,Guid> getService, 
        CreateAgentViewRequest request)
    {
        return getService.GetAsync(new GetAndAddAgentQuery
        {
            Username = request.Username,
            BrandId = request.BrandId,
            LevelType = request.LevelType,
            CardGameRebatePercentage = request.CardGameRebatePercentage,
            CommissionPercentage = request.CommissionPercentage,
            DividendPercentage = request.DividendPercentage,
            LiveDealerRebatePercentage = request.LiveDealerRebatePercentage,
            LotteryRebatePercentage = request.LotteryRebatePercentage,
            SlotRebatePercentage = request.SlotRebatePercentage,
            SportsRebatePercentage = request.SportsRebatePercentage,
            VirtualSportsRebatePercentage = request.VirtualSportsRebatePercentage,
            ParentAgentId = request.ParentAgentId,
            Password = request.Password,
            RealName = request.RealName,
            Type = request.Type,
            Remarks = request.Remarks,
            EditorUsername = request.EditorUsername
        });
    }
}