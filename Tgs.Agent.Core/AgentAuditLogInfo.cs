using Tgs.Agent.Core.Models.Enums;

namespace Tgs.Agent.Core;
public record AgentAuditLogInfo(
    Guid Id,
    AgentOperation Operation,
    string CreatedByUsername,
    DateTimeOffset CreatedOn);