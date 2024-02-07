using Tgs.Agent.Client.Models.Enums;

namespace Tgs.Agent.Client.Models;

public record CreateAgentByPlayerUpgradeRequest(
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
    Guid? ParentAgentId,
    string? Remarks,
    bool NegativeAgentDividendCarryForward = true);