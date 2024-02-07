using Tgs.Agent.Core.Models.Enums;

namespace Tgs.Agent.Core.ApplicationServices;
public record SetAgentStatusCommand(
    Guid AgentId,
    AgentStatus Status,
    string? ModifyUserId);
