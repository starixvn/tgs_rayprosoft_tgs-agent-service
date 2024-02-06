using Tgs.Agent.Core;

namespace Tgs.Agent.Service.ViewModels;

public record CreateAgentByPlayerUpgradeViewRequest(
    Guid BrandId,
    Guid PlayerWalletId,
    string Username,
    string? RealName,
    string PlayerCode,
    string CurrencyCode,
    string? Phone,
    string? Email,
    string EditorUsername,
    AgentLevelType LevelType,
    AgentType Type,
    bool NegativeAgentDividendCarryForward,
    Guid? ParentAgentId,
    string? Remarks);