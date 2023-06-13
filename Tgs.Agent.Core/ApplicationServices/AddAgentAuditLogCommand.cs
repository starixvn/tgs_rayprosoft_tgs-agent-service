using Tgs.Agent.Core.Models.Enums;

namespace Tgs.Agent.Core.ApplicationServices;
public record AddAgentAuditLogCommand(
    Guid AgentId,
    string EditUsername,
    AgentOperation Operation);