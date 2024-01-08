namespace Tgs.Agent.Core.ApplicationServices;

public record AddAgentByPlayerUpgradeQuery
{
    public string Username { get; init; } = default!;
    public string PlayerCode { get; init; } = default!;
    public Guid BrandId { get; init; }
    public Guid? BrandContractId { get; init; }
    public Guid? ParentAgentId { get; init; }
    public AgentLevelType LevelType { get; init; }
    public AgentType Type { get; init; }
    public string CurrencyCode { get; init; } = default!;
    public string? RealName { get; init; } = default!;
    public string? Email { get; init; } = default!;
    public string? Phone { get; init; } = default!;
    public string? Remarks { get; init; } = default!;
    public string EditorUsername { get; set; } = default!;
}