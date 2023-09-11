namespace Tgs.Agent.Core.ApplicationServices;

public record ClearNegativeCommissionCommand(Guid AgentId ,Guid ClearByUserId);