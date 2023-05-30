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
    short Status,
    short Type,
    int AgentLevelType,
    decimal DividendPercentage,
    int PlayerNumber,
    decimal AccumulatedProfitSharing);