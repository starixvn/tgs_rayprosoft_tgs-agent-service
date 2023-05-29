using Microsoft.AspNetCore.Mvc;
using Tgs.Agent.Core.ApplicationServices;
using Tgs.ServiceUtility;
using Tgs.Agent.Service.ViewModels;

namespace Tgs.Agent.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SubAgentController : ControllerBase
{
    [HttpPost("resetPassword", Name = "ResetSubAgentPassword")]
    public ValueTask ResetSubAgentPasswordAsync(
        [FromServices] ICommandService<ResetSubAgentPasswordCommand> commandService,
        [FromBody] ResetSubAgentPasswordViewModel request)
        => commandService.ExecuteAsync(
            new(
                request.AgentId,
                request.SubAgentId,
                request.Password),
            HttpContext.RequestAborted);

    [HttpPost("setInfo", Name = "SetAgentInfo")]
    public ValueTask SetAgentInfoAsync(
        [FromServices] ICommandService<SetAgentInfoCommand> commandService,
        [FromBody] SetAgentInfoViewModel request)
        => commandService.ExecuteAsync(
            new SetAgentInfoCommand(
                request.AgentId,
                request.SubAgentId,
                request.RealName,
                request.Email,
                request.Phone),
            HttpContext.RequestAborted);
}