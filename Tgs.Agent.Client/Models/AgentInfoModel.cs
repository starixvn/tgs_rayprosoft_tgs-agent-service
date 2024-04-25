using Tgs.Agent.Client.Models.Enums;

namespace Tgs.Agent.Client.Models;

public record AgentInfoModel(
    Guid Id,
    string Username,
    string? RealName,
    string? Phone,
    string? Email,
    string? QQ,
    string? Wechat,
    string InvitationCode,
    Guid? ParentAgentId,
    DateTimeOffset RegisteredOn,
    AgentStatus Status,
    AgentType Type,
    AgentLevelType AgentLevelType,
    decimal DividendPercentage,
    int PlayerNumber,
    decimal AccumulatedProfitSharing,
    Guid BrandId,
    Guid DefaultWalletId,
    Guid? WalletId,
    bool IsSetPayPassword,
    bool NegativeAgentDividendCarryForward);