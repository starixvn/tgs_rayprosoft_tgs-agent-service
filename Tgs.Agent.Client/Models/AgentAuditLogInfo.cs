using Tgs.Agent.Client.Models.Enums;

namespace Tgs.Agent.Client.Models;

public class AgentAuditLogInfo
{
    public Guid Id { get; set; }
    public AgentOperation Operation { get; set; }

    public string CreatedByUsername { get; set; } = default!;
    public DateTimeOffset CreatedOn { get; set; }
}