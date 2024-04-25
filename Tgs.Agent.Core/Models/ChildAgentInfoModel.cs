using Tgs.Agent.Core.Models.Enums;

namespace Tgs.Agent.Core.Models;

public class ChildAgentInfoModel
{
    public Guid AgentId { get; set; }
    public Guid ParentAgentId { get; set; }
    public string AgentUsername { get; set; } = default!;
    public string? RealName { get; set; }
    public AgentStatus Status { get; set; }
    public decimal DividendPercentage { get; set; }
    public DateTime? RegisteredOn { get; set; }
}