using Tgs.Agent.Core.Models.Enums;

namespace Tgs.Agent.Service.ViewModels;

public record AgentAuditLogInfoViewModel(
    Guid Id,
    AgentOperation Operation,
    string CreatedByUsername,
    DateTimeOffset CreatedOn);