using Tgs.Agent.Client.Models.Enums;

namespace Tgs.Agent.Client.Models;
public record AddAgentAuditLogRequest(
    Guid AgentId,
    AgentOperation Operation,
    string EditUsername);