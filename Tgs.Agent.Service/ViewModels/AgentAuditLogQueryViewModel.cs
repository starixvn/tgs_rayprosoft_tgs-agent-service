using Microsoft.AspNetCore.Mvc;

namespace Tgs.Agent.Service.ViewModels;

public class AgentAuditLogQueryViewModel
{
    [FromQuery(Name = "agent_id")]
    public Guid AgentId { get; set; }

    [FromQuery(Name = "from")]
    public DateTimeOffset From { get; set; }

    [FromQuery(Name = "to")]
    public DateTimeOffset To { get; set; }
}