using Tgs.Agent.Core;

namespace Tgs.Agent.Service.ViewModels;

public record CreateAgentByPlayerUpgradeViewRequest(
    Guid BrandId,
    string Username,
    string? RealName,
    string PlayerCode,
    string CurrencyCode,
    string? Phone,
    string? Email,
    string EditorUsername,
    AgentLevelType LevelType,
    AgentType Type,
    Guid? ParentAgentId,
    string? Remarks);