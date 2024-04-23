using Tgs.Agent.Core.Models.Enums;

namespace Tgs.Agent.Core.ApplicationServices;

public record AddAgentQuery
{
    public string Username { get; init; } = default!;
    public Guid BrandId { get; init; }
    public Guid? ParentAgentId { get; init; }
    public bool NegativeAgentDividendCarryForward { get; init; }
    public AgentLevelType LevelType { get; init; }
    public AgentType Type { get; init; }
    public string CurrencyCode { get; init; } = default!;
    public decimal CommissionPercentage { get; init; }
    public decimal DividendPercentage { get; init; }
    public string Password { get; init; } = default!;
    public string RealName { get; init; } = default!;
    public string Email { get; init; } = default!;

    public string Phone { get; init; } = default!;
    public string Remarks { get; init; } = default!;
    public decimal GradeIntervalLimit { get; init; }
    public string EditorUsername { get; set; } = default!;
}