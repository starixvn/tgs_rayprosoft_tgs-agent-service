namespace Tgs.Agent.Client.Models;
public record SetAgentPayPasswordRequest(
    Guid AgentId,
    string PayPassword,
    string? ModifyUserId);