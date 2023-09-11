namespace Tgs.Agent.Client.Models;

public record ClearAgentNegativeCommissionRequest(Guid AgentId, Guid ClearByUserId);