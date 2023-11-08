using Microsoft.AspNetCore.Mvc;
using Tgs.Agent.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.Agent.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AgentDividendConditionController : ControllerBase
{
    [HttpDelete("{agentId}")]
    public ValueTask ClearAgentDividendConditionsAsync(
        [FromServices] ICommandService<ClearDividendConditionsCommand> commandService,
        Guid agentId)
        => commandService.ExecuteAsync(
            new ClearDividendConditionsCommand(agentId),
            HttpContext.RequestAborted);
}