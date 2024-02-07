using Tgs.Agent.Core;
using Tgs.Agent.Core.Models.Enums;

namespace Tgs.Agent.Service.ViewModels;

public record CreateAgentByPlayerUpgradeViewRequest(
    Guid BrandId,
    Guid PlayerWalletId,
    byte[] Password,
    byte[] Salt,
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