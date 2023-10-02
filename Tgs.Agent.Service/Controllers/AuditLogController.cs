using Microsoft.AspNetCore.Mvc;
using Tgs.Agent.Core;
using Tgs.Agent.Core.ApplicationServices;
using Tgs.Agent.Service.ViewModels;
using Tgs.Paging;
using Tgs.ServiceUtility;

namespace Tgs.Agent.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuditLogController : ControllerBase
{
    [HttpPost(Name = "AddAgentAuditLog")]
    public ValueTask AddAgentAuditLogAsync(
        [FromServices] ICommandService<AddAgentAuditLogCommand> commandService,
        [FromBody] AgentAuditLogViewModel request)
        => commandService.ExecuteAsync(
                new(
                    request.AgentId,
                    request.EditUsername,
                    request.Operation),
            HttpContext.RequestAborted);

    [HttpGet("{brandId}", Name = "GetAgentAuditLog")]
    public async Task<PagingResponseViewModel<AgentAuditLogInfoViewModel>> GetAgentAuditLogAsync(
        [FromServices] IQueryService<GetAgentAuditLogQuery, AgentAuditLogInfo> queryService,
        [FromServices] IGetService<GetAgentAuditLogQuery, int> countService,
        Guid brandId,
        [FromQuery] AgentAuditLogQueryViewModel logQuery,
        [FromQuery] PagingViewModel paging)
    {
        var sourcePaging = new Paging<AgentAuditLogInfo>
        {
            Page = paging.Page ?? 1,
            RowCount = paging.Rows ?? 10
        };

        if (paging.ShowAll == true)
        {
            sourcePaging.Page = -1;
            sourcePaging.RowCount = -1;
        }

        if (!string.IsNullOrWhiteSpace(paging.SortBy))
            sourcePaging.SortByPropertyName(paging.SortBy, paging.IsDesc ?? false);

        var query = new GetAgentAuditLogQuery(brandId, logQuery.AgentId, logQuery.From, logQuery.To, sourcePaging);

        var totalCount = await countService.GetAsync(query, HttpContext.RequestAborted).ConfigureAwait(false);

        var items = queryService.QueryAsync(query, HttpContext.RequestAborted)
            .Select(row => new AgentAuditLogInfoViewModel(
                row.Id,
                row.Operation,
                row.CreatedByUsername,
                row.CreatedOn));

        return new PagingResponseViewModel<AgentAuditLogInfoViewModel>
        {
            TotalCount = totalCount,
            Page = paging.Page.GetValueOrDefault(1),
            Rows = paging.Rows.GetValueOrDefault(10),
            Items = items.ToEnumerable(),
        };
    }
}