namespace Tgs.Agent.Service.Models;

public record AgentGroupCountInfo
{
    public int DirectAgentCount { get; set; }
    public int IndirectAgentCount { get; set; }
    public int DirectPlayerCount { get; set; }
    public int IndirectPlayerCount { get; set; }
}