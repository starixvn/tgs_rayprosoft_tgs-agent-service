using Tgs.Agent.Client.Models.Enums;

namespace Tgs.Agent.Client.Models;
public record SetAgentInfoRequest(
    Guid AgentId,
    string? RealName);