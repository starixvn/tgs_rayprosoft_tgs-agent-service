using Microsoft.EntityFrameworkCore;

namespace Tgs.Agent.Repositories.Entities;

public partial class TgsContext : DbContext
{
    public TgsContext()
    {
    }

    public Task AgentUpdatePathAsync(
        Guid agentId,
        int agentNLevel,
        string agentPath,
        string newAgentPath)
    {
        return Task.Run(() => TgsDbFunctions.AgentUpdatePath(agentId, agentNLevel, agentPath, newAgentPath));
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        ////var methodInfo = typeof(TgsContext).GetMethod(nameof(AgentUpdatePathAsync));
        //_ = modelBuilder
        //    .HasDbFunction(() => TgsDbFunctions.AgentUpdatePath(default, default, default!, default!))
        //    .HasSchema("dbo")
        //    .HasName("agent_update_path");
    }
}