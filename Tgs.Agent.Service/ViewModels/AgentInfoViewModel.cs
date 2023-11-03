using Tgs.Agent.Core;

namespace Tgs.Agent.Service.ViewModels;

public record AgentInfoViewModel(
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
    Guid BrandId);