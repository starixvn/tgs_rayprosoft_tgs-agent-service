using Microsoft.AspNetCore.Mvc;

namespace Tgs.Agent.Service.ViewModels;

public class AgentHistoryQueryViewModel
{
    [FromQuery(Name = "from")]
    public DateTime From { get; set; }

    [FromQuery(Name = "to")]
    public DateTime To { get; set; }
}