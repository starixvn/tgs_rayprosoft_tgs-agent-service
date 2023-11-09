namespace Tgs.Agent.Core.Models;
public record AgentPathInfo(
    Guid AgentId,
    int NLevel,
    string Path);