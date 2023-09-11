using Microsoft.AspNetCore.Mvc;
using Tgs.Agent.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.Agent.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AgentCommissionController : ControllerBase
{
    [HttpPut("clear/{agentId}/{clearByUserId}")]
    public ValueTask ClearNegativeCommissionAsync(
        [FromServices] ICommandService<ClearNegativeCommissionCommand> commandService,
        Guid agentId,
        Guid clearByUserId)
        => commandService.ExecuteAsync(
            new(
                agentId,
                clearByUserId),
            HttpContext.RequestAborted);
}
