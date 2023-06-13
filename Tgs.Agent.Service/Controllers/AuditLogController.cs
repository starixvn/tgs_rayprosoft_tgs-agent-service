using Microsoft.AspNetCore.Mvc;
using Tgs.Agent.Core.ApplicationServices;
using Tgs.Agent.Service.ViewModels;
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
}