namespace Tgs.Agent.Client.Models;

public class GetAgentAuditLogQueryConditions
{
    public Guid BrandId { get; set; }
    public Guid AgentId { get; set; }
    public DateTimeOffset From { get; set; }
    public DateTimeOffset To { get; set; }
}