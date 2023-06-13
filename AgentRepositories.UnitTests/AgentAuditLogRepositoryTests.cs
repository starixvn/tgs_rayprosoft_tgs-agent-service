using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Tgs.Agent.Core.Models.Enums;
using Tgs.Agent.Repositories;
using Tgs.Agent.Repositories.Entities;

namespace AgentRepositories.UnitTests;

public class AgentAuditLogRepositoryTests
{
    [Test]
    public async Task Add_Agent_Audit_Log()
    {
        var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsContext>();
        _ = dbContextOptionsBuilder
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(
                InMemoryEventId.TransactionIgnoredWarning));

        var dbContext = new TgsContext(dbContextOptionsBuilder.Options);

        var agentId = Guid.Parse("8480c811-879b-41e0-8b8b-bd0c9cdbdf31");
        var operation = AgentOperation.Info;
        var id = Guid.Parse("7109b9ed-89f5-4f88-36b5-08d98d5fec0a");
        var editUsername = "editUsername";

        var agentAuditLog = new AgentAuditLog
        {
            Id = id,
            AgentId = agentId,
            CreatedByUsername = editUsername,
            CreatedOn = DateTime.UtcNow,
            Operation = (short)operation,
        };

        _ = await dbContext.AgentAuditLogs.AddAsync(agentAuditLog);
        _ = await dbContext.SaveChangesAsync();

        var sut = new AgentAuditLogRepository(dbContext);

        await sut.AddAgentAuditLogAsync(agentId, operation, editUsername);

        var actual = await dbContext.AgentAuditLogs.FirstOrDefaultAsync(x => x.Id == id);

        Assert.NotNull(actual);
        Assert.AreEqual(actual!.AgentId, agentId);
        Assert.AreEqual(actual!.CreatedByUsername, editUsername);
    }
}