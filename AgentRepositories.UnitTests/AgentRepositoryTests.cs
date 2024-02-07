using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;
using Tgs.Agent.Core;
using Tgs.Agent.Core.ApplicationServices;
using Tgs.Agent.Core.Models;
using Tgs.Agent.Core.Models.Enums;
using Tgs.Agent.Repositories;
using Tgs.Agent.Repositories.Entities;

namespace AgentRepositories.UnitTests;

public class AgentRepositoryTests
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

        var sut = new AgentRepository(dbContext, NullLogger<AgentRepository>.Instance);

        await sut.AddAgentAuditLogAsync(agentId, operation, editUsername);

        var actual = await dbContext.AgentAuditLogs.FirstOrDefaultAsync(x => x.Id == id);

        Assert.NotNull(actual);
        Assert.AreEqual(actual!.AgentId, agentId);
        Assert.AreEqual(actual!.CreatedByUsername, editUsername);
    }

    [Test]
    public async Task Get_Agent_By_Id()
    {
        var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsContext>();
        _ = dbContextOptionsBuilder
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(
                InMemoryEventId.TransactionIgnoredWarning));

        var dbContext = new TgsContext(dbContextOptionsBuilder.Options);

        var agentId = Guid.Parse("30945e96-a762-4b6e-9acb-0266727528d7");

        var agent = new Agent
        {
            Id = agentId,
            Password = Encoding.UTF8.GetBytes("123"),
            Salt = Encoding.UTF8.GetBytes("456"),
            InvitationCode = "5kiqhx",
            Username = "username",
            WithdrawPassword = Array.Empty<byte>(),
            WithdrawSalt = Array.Empty<byte>(),
            ParentAgentId = Guid.Parse("8480c811-879b-41e0-8b8b-bd0c9cdbdf31"),
            RealName = "realName",
            Phone = "phone",
            Email = "email",
            RegisteredOn = new DateTime(2000, 1, 1),
        };

        _ = await dbContext.Agents.AddAsync(agent);
        _ = await dbContext.SaveChangesAsync();

        var sut = new AgentRepository(dbContext, NullLogger<AgentRepository>.Instance);

        var actual = await sut.GetAgentByIdAsync(agentId);

        Assert.NotNull(actual);
        Assert.AreEqual(actual!.Id, agentId);
    }

    [Test]
    public async Task Get_Agent_By_Username()
    {
        var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsContext>();
        _ = dbContextOptionsBuilder
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(
                InMemoryEventId.TransactionIgnoredWarning));

        var dbContext = new TgsContext(dbContextOptionsBuilder.Options);

        var brandId = Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af");
        var username = "username";

        var agent = new Agent
        {
            Id = Guid.Parse("30945e96-a762-4b6e-9acb-0266727528d7"),
            BrandId = brandId,
            Password = Encoding.UTF8.GetBytes("123"),
            Salt = Encoding.UTF8.GetBytes("456"),
            InvitationCode = "5kiqhx",
            Username = "username",
            WithdrawPassword = Array.Empty<byte>(),
            WithdrawSalt = Array.Empty<byte>(),
            ParentAgentId = Guid.Parse("8480c811-879b-41e0-8b8b-bd0c9cdbdf31"),
            RealName = "realName",
            Phone = "phone",
            Email = "email",
            RegisteredOn = new DateTime(2000, 1, 1),
        };

        _ = await dbContext.Agents.AddAsync(agent);
        _ = await dbContext.SaveChangesAsync();

        var sut = new AgentRepository(dbContext, NullLogger<AgentRepository>.Instance);

        var actual = await sut.GetAgentByUsernameAsync(brandId, username);

        Assert.NotNull(actual);
        Assert.AreEqual(actual!.Username, username);
    }

    [Test]
    public async Task Set_Agent_Dividend_Percentage()
    {
        var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsContext>();
        _ = dbContextOptionsBuilder
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(
                InMemoryEventId.TransactionIgnoredWarning));

        var dbContext = new TgsContext(dbContextOptionsBuilder.Options);

        var agentId = Guid.Parse("8480c811-879b-41e0-8b8b-bd0c9cdbdf31");

        var agent = new Agent
        {
            Id = agentId,
            Password = Encoding.UTF8.GetBytes("123"),
            Salt = Encoding.UTF8.GetBytes("456"),
            InvitationCode = "5kiqhx",
            Username = "username",
            WithdrawPassword = Array.Empty<byte>(),
            WithdrawSalt = Array.Empty<byte>(),
            DividendPercentage = 70M
        };

        _ = await dbContext.Agents.AddAsync(agent);
        _ = await dbContext.SaveChangesAsync();

        var sut = new AgentRepository(dbContext, NullLogger<AgentRepository>.Instance);

        await sut.SetAgentDividendPercentageAsync(agentId, 68M);

        var actual = await dbContext.Agents.FirstOrDefaultAsync(x => x.Id == agentId);

        Assert.NotNull(actual);
        Assert.AreEqual(actual!.DividendPercentage, 68M);
    }

    [Test]
    public async Task Set_Agent_Info()
    {
        var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsContext>();
        _ = dbContextOptionsBuilder
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(
                InMemoryEventId.TransactionIgnoredWarning));

        var dbContext = new TgsContext(dbContextOptionsBuilder.Options);

        var agentId = Guid.Parse("8480c811-879b-41e0-8b8b-bd0c9cdbdf31");
        var parentAgentId = Guid.NewGuid();
        var realName = "realName";

        var agent = new Agent
        {
            Id = agentId,
            Password = Encoding.UTF8.GetBytes("123"),
            Salt = Encoding.UTF8.GetBytes("456"),
            InvitationCode = "5kiqhx",
            Username = "username",
            WithdrawPassword = Array.Empty<byte>(),
            WithdrawSalt = Array.Empty<byte>(),
            RealName = "abc",
            Email = "abc",
            Phone = "abc",
        };

        _ = await dbContext.Agents.AddAsync(agent);
        _ = await dbContext.SaveChangesAsync();

        var sut = new AgentRepository(dbContext, NullLogger<AgentRepository>.Instance);

        var command = new UpdateAgentModel(
            agentId,
            parentAgentId,
            AgentStatus.Enabled,
            AgentLevelType.Agent,
            AgentConfigStatus.WaitForConfigurePercentage,
            true,
            realName,
            null,
            "editor");

        await sut.SetAgentInfoAsync(command);

        var actual = dbContext.Agents.First(x => x.Id == agentId);

        Assert.NotNull(actual);
        Assert.AreEqual(actual.RealName, realName);
        Assert.That(actual.Status, Is.EqualTo((short)AgentStatus.Enabled));
        Assert.That(actual.LevelType, Is.EqualTo((short)AgentLevelType.Agent));
        Assert.That(actual.AgentStatus, Is.EqualTo((short)AgentConfigStatus.WaitForConfigurePercentage));
        Assert.That(actual.ParentAgentId, Is.EqualTo(parentAgentId));
        Assert.That(actual.Remarks, Is.EqualTo(null));
    }

    [Test]
    public async Task Set_Agent_Contact()
    {
        var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsContext>();
        _ = dbContextOptionsBuilder
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(
                InMemoryEventId.TransactionIgnoredWarning));

        var dbContext = new TgsContext(dbContextOptionsBuilder.Options);

        var agentId = Guid.Parse("8480c811-879b-41e0-8b8b-bd0c9cdbdf31");
        var email = "email";
        var phone = "phone";

        var agent = new Agent
        {
            Id = agentId,
            Password = Encoding.UTF8.GetBytes("123"),
            Salt = Encoding.UTF8.GetBytes("456"),
            InvitationCode = "5kiqhx",
            Username = "username",
            WithdrawPassword = Array.Empty<byte>(),
            WithdrawSalt = Array.Empty<byte>(),
            RealName = "abc",
            Email = "abc",
            Phone = "abc",
        };

        _ = await dbContext.Agents.AddAsync(agent);
        _ = await dbContext.SaveChangesAsync();

        var sut = new AgentRepository(dbContext, NullLogger<AgentRepository>.Instance);

        await sut.SetAgentContactAsync(agentId, email, phone);

        var actual = await dbContext.Agents.FirstOrDefaultAsync(x => x.Id == agentId);

        Assert.NotNull(actual);
        Assert.AreEqual(actual!.Email, email);
        Assert.AreEqual(actual!.Phone, phone);
    }

    [Test]
    public async Task Set_Password()
    {
        var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsContext>();
        _ = dbContextOptionsBuilder
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(
                InMemoryEventId.TransactionIgnoredWarning));

        var dbContext = new TgsContext(dbContextOptionsBuilder.Options);

        var agentId = Guid.Parse("8480c811-879b-41e0-8b8b-bd0c9cdbdf31");
        var agent = new Agent
        {
            Id = agentId,
            Password = Encoding.UTF8.GetBytes("123"),
            Salt = Encoding.UTF8.GetBytes("456"),
            InvitationCode = "5kiqhx",
            Username = "username",
            WithdrawPassword = Array.Empty<byte>(),
            WithdrawSalt = Array.Empty<byte>(),
        };

        _ = await dbContext.Agents.AddAsync(agent);
        _ = await dbContext.SaveChangesAsync();

        var sut = new AgentRepository(dbContext, NullLogger<AgentRepository>.Instance);

        var newPassword = Encoding.UTF8.GetBytes("123456");
        var newSalt = Encoding.UTF8.GetBytes("456789");

        await sut.SetPasswordAsync(agentId, newPassword, newSalt);

        var actual = await dbContext.Agents.FirstOrDefaultAsync(x => x.Id == agentId);

        Assert.NotNull(actual);
        Assert.AreEqual(actual!.Password, newPassword);
        Assert.AreEqual(actual!.Salt, newSalt);
    }

    [Test]
    public async Task 更新代理提款密碼()
    {
        var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsContext>();
        _ = dbContextOptionsBuilder
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(
                InMemoryEventId.TransactionIgnoredWarning));

        var dbContext = new TgsContext(dbContextOptionsBuilder.Options);

        var agentId = Guid.Parse("8480c811-879b-41e0-8b8b-bd0c9cdbdf31");
        var agent = new Agent
        {
            Id = agentId,
            Password = Encoding.UTF8.GetBytes("123"),
            Salt = Encoding.UTF8.GetBytes("456"),
            InvitationCode = "5kiqhx",
            Username = "username",
            WithdrawPassword = Encoding.UTF8.GetBytes("123"),
            WithdrawSalt = Encoding.UTF8.GetBytes("456"),
        };

        _ = await dbContext.Agents.AddAsync(agent);
        _ = await dbContext.SaveChangesAsync();

        var sut = new AgentRepository(dbContext, NullLogger<AgentRepository>.Instance);

        var newPassword = Encoding.UTF8.GetBytes("123456");
        var newSalt = Encoding.UTF8.GetBytes("456789");

        await sut.SetPayPasswordAsync(agentId, newPassword, newSalt);

        var actual = await dbContext.Agents.FirstOrDefaultAsync(x => x.Id == agentId);

        Assert.NotNull(actual);
        Assert.AreEqual(actual!.WithdrawPassword, newPassword);
        Assert.AreEqual(actual!.WithdrawSalt, newSalt);
    }

    [Test]
    public async Task 查詢代理操作紀錄()
    {
        var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsContext>();
        _ = dbContextOptionsBuilder
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(
                InMemoryEventId.TransactionIgnoredWarning));

        var dbContext = new TgsContext(dbContextOptionsBuilder.Options);

        var brandId = Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af");
        var agentId = Guid.Parse("8480c811-879b-41e0-8b8b-bd0c9cdbdf31");
        var agent = new Agent
        {
            Id = agentId,
            BrandId = brandId,
            Password = Encoding.UTF8.GetBytes("123"),
            Salt = Encoding.UTF8.GetBytes("456"),
            InvitationCode = "5kiqhx",
            Username = "username",
            WithdrawPassword = Encoding.UTF8.GetBytes("123"),
            WithdrawSalt = Encoding.UTF8.GetBytes("456"),
        };

        _ = await dbContext.Agents.AddAsync(agent);

        var log = new AgentAuditLog
        {
            Id = Guid.NewGuid(),
            AgentId = agentId,
            CreatedByUsername = "username",
            CreatedOn = DateTime.Now.AddDays(-1d),
            Operation = 6
        };

        _ = await dbContext.AgentAuditLogs.AddAsync(log);
        _ = await dbContext.SaveChangesAsync();

        var sut = new AgentRepository(dbContext, NullLogger<AgentRepository>.Instance);

        var newPassword = Encoding.UTF8.GetBytes("123456");
        var newSalt = Encoding.UTF8.GetBytes("456789");

        var conditions = new Tgs.Agent.Core.AgenAuditLogQueryConditions(
            brandId,
            agentId,
            DateTimeOffset.Now.AddDays(-14),
            DateTimeOffset.Now);

        var paging = new Tgs.Paging.Paging<Tgs.Agent.Core.AgentAuditLogInfo>
        {
            IsDesc = false,
            Page = 1,
            RowCount = 20,
        };
        var actual = await sut.QueryAgentAuditLogsAsync(conditions, paging).ToArrayAsync();

        Assert.AreEqual(1, actual.Length);
        Assert.AreEqual(AgentOperation.PayPassword, actual[0].Operation);
        Assert.AreEqual("username", actual[0].CreatedByUsername);
    }

    [Test]
    public async Task 查詢代理操作紀錄總筆數()
    {
        var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsContext>();
        _ = dbContextOptionsBuilder
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(
                InMemoryEventId.TransactionIgnoredWarning));

        var dbContext = new TgsContext(dbContextOptionsBuilder.Options);

        var brandId = Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af");
        var agentId = Guid.Parse("8480c811-879b-41e0-8b8b-bd0c9cdbdf31");
        var agent = new Agent
        {
            Id = agentId,
            BrandId = brandId,
            Password = Encoding.UTF8.GetBytes("123"),
            Salt = Encoding.UTF8.GetBytes("456"),
            InvitationCode = "5kiqhx",
            Username = "username",
            WithdrawPassword = Encoding.UTF8.GetBytes("123"),
            WithdrawSalt = Encoding.UTF8.GetBytes("456"),
        };

        _ = await dbContext.Agents.AddAsync(agent);

        var log = new AgentAuditLog
        {
            Id = Guid.NewGuid(),
            AgentId = agentId,
            CreatedByUsername = "username",
            CreatedOn = DateTime.Now.AddDays(-1d),
            Operation = 6
        };

        _ = await dbContext.AgentAuditLogs.AddAsync(log);
        _ = await dbContext.SaveChangesAsync();

        var sut = new AgentRepository(dbContext, NullLogger<AgentRepository>.Instance);

        var newPassword = Encoding.UTF8.GetBytes("123456");
        var newSalt = Encoding.UTF8.GetBytes("456789");

        var conditions = new Tgs.Agent.Core.AgenAuditLogQueryConditions(
            brandId,
            agentId,
            DateTimeOffset.Now.AddDays(-14),
            DateTimeOffset.Now);

        var paging = new Tgs.Paging.Paging<Tgs.Agent.Core.AgentAuditLogInfo>
        {
            IsDesc = false,
            Page = 1,
            RowCount = 20,
        };
        var actual = await sut.CountAgentAuditLogsAsync(conditions);

        Assert.AreEqual(1, actual);
    }

    [Test]
    public async Task 查詢Agent_Event_Source()
    {
        var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsContext>();
        _ = dbContextOptionsBuilder
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(
                InMemoryEventId.TransactionIgnoredWarning));

        var dbContext = new TgsContext(dbContextOptionsBuilder.Options);
        var eventId = Guid.NewGuid();
        var agentId = Guid.Parse("8480c811-879b-41e0-8b8b-bd0c9cdbdf31");

        var agentEvent = new AgentEventSource
        {
            Id = eventId,
            Action = EventAction.Create.ToString(),
            ChangeSets = "{}",
            CreatedOn = DateTime.UtcNow.AddDays(-1),
            BrandId = Guid.NewGuid(),
            AgentId = agentId,
        };

        _ = await dbContext.AgentEventSources.AddAsync(agentEvent);
        _ = await dbContext.SaveChangesAsync();

        var sut = new AgentRepository(dbContext, NullLogger<AgentRepository>.Instance);

        var newPassword = Encoding.UTF8.GetBytes("123456");
        var newSalt = Encoding.UTF8.GetBytes("456789");

        var actual = await sut
            .QueryAgentEventAsync(new AgentEventQueryConditions(agentId, DateTime.Now.AddDays(-14), DateTime.Now))
            .ToArrayAsync();

        Assert.That(actual, Has.Length.EqualTo(1));
        Assert.That(actual[0].EventId, Is.EqualTo(eventId));
    }

    [Test]
    public async Task 查詢Agent_Event_Source細項()
    {
        var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsContext>();
        _ = dbContextOptionsBuilder
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(
                InMemoryEventId.TransactionIgnoredWarning));

        var dbContext = new TgsContext(dbContextOptionsBuilder.Options);

        var eventId = Guid.Parse("018a218f-b4cb-0000-0001-07e7e07dc945");
        var brandId = Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af");
        var agentId = Guid.Parse("8480c811-879b-41e0-8b8b-bd0c9cdbdf31");

        _ = await dbContext.AgentEventSources.AddAsync(new AgentEventSource
        {
            Id = eventId,
            Action = EventAction.Create.ToString(),
            BrandId = brandId,
            ChangeSets = "{}",
            CreatedOn = DateTime.UtcNow,
            AgentId = agentId
        });
        _ = await dbContext.SaveChangesAsync();

        var sut = new AgentRepository(dbContext, NullLogger<AgentRepository>.Instance);

        var actual = await sut.GetAgentEventAsync(eventId);

        Assert.That(actual, Is.Not.Null);
        Assert.That(actual!.Action, Is.EqualTo(EventAction.Create));
    }

    [Test]
    public async Task 更新Agent_Snapshot_Id()
    {
        var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsContext>();
        _ = dbContextOptionsBuilder
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(
                InMemoryEventId.TransactionIgnoredWarning));

        var dbContext = new TgsContext(dbContextOptionsBuilder.Options);

        var eventId = Guid.Parse("01852e47-a829-0000-0001-07e6af979b15");
        var brandId = Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af");
        var agentId = Guid.Parse("8480c811-879b-41e0-8b8b-bd0c9cdbdf31");

        var agent = new Agent
        {
            Id = agentId,
            Password = Array.Empty<byte>(),
            Salt = Array.Empty<byte>(),
            Username = "username",
            WithdrawPassword = Array.Empty<byte>(),
            WithdrawSalt = Array.Empty<byte>(),
            SnapshotEventId = Guid.Parse("01852e47-a6ee-0000-0001-07e6af979b15"),
            InvitationCode = "5kiqhx"
        };

        _ = await dbContext.Agents
            .AddAsync(agent)
            .ConfigureAwait(false);

        _ = await dbContext.SaveChangesAsync();

        var sut = new AgentRepository(dbContext, NullLogger<AgentRepository>.Instance);

        await sut.UpdateAgentSnapshotAsync(
            agentId,
            eventId,
            DateTime.UtcNow);

        var actual = await dbContext.Agents.FirstAsync(x => x.Id == agentId);

        Assert.That(actual.SnapshotEventId, Is.EqualTo(eventId));
    }

    [Test]
    public async Task 新增Agent_Event_Source()
    {
        var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsContext>();
        _ = dbContextOptionsBuilder
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(
                InMemoryEventId.TransactionIgnoredWarning));

        var dbContext = new TgsContext(dbContextOptionsBuilder.Options);

        var eventId = Guid.Parse("018a218f-b4cb-0000-0001-07e7e07dc945");
        var brandId = Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af");
        var agentId = Guid.Parse("8480c811-879b-41e0-8b8b-bd0c9cdbdf31");

        var sut = new AgentRepository(dbContext, NullLogger<AgentRepository>.Instance);

        await sut.CreateAgentEventAsync(
            eventId,
            brandId,
            agentId,
            EventAction.Create,
            JsonDocument.Parse("{}"),
            "50ce61eb-8c9d-4f84-3cf6-08d9262ce1a5");

        var actual = await dbContext.AgentEventSources.FirstAsync(x => x.Id == eventId);

        Assert.That(actual.Id, Is.EqualTo(eventId));
    }

    [Test]
    public async Task 變更代理狀態()
    {
        var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsContext>();
        _ = dbContextOptionsBuilder
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(
                InMemoryEventId.TransactionIgnoredWarning));

        var dbContext = new TgsContext(dbContextOptionsBuilder.Options);
        var agentId = Guid.Parse("8480c811-879b-41e0-8b8b-bd0c9cdbdf31");

        _ = await dbContext.Agents.AddAsync(new Agent()
        {
            Id = agentId,
            Status = (short)AgentStatus.Enabled,
            InvitationCode = "jhmdpi",
            Password = Encoding.UTF8.GetBytes("password"),
            Salt = Encoding.UTF8.GetBytes("salt"),
            Username = "agentUsername",
            WithdrawPassword = Array.Empty<byte>(),
            WithdrawSalt = Array.Empty<byte>()
        });

        var sut = new AgentRepository(dbContext, NullLogger<AgentRepository>.Instance);

        await sut.SetAgentStatusAsync(
            agentId,
            AgentStatus.InBlacklist);

        var actual = await dbContext.Agents.FirstAsync(x => x.Id == agentId);

        Assert.AreEqual((short)AgentStatus.InBlacklist, actual.Status);
    }

    [Test]
    public async Task 清除代理階梯式分紅設置()
    {
        var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsContext>();
        _ = dbContextOptionsBuilder
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(
                InMemoryEventId.TransactionIgnoredWarning));

        var dbContext = new TgsContext(dbContextOptionsBuilder.Options);

        var agentId = Guid.NewGuid();

        await dbContext.AgentDividendConditions.AddRangeAsync(new AgentDividendCondition[] {
            new AgentDividendCondition()
            {
                AgentId = agentId,
                ActiveNumber = 10,
                DividendPercentage = 15m,
                NetProfit = 10m,
                Rank = 1,
            },
            new AgentDividendCondition()
            {
                AgentId = agentId,
                ActiveNumber = 20,
                DividendPercentage = 18m,
                NetProfit = 20m,
                Rank = 2,
            },
            new AgentDividendCondition()
            {
                AgentId = agentId,
                ActiveNumber = 100,
                DividendPercentage = 20m,
                NetProfit = 100m,
                Rank = 3,
            }
        });

        _ = await dbContext.SaveChangesAsync();

        var sut = new AgentRepository(dbContext, NullLogger<AgentRepository>.Instance);

        await sut.ClearAgentDividendConditionsAsync(agentId);

        var actual = await dbContext.AgentDividendConditions
            .Where(x => x.AgentId == agentId)
            .ToArrayAsync();

        Assert.AreEqual(0, actual.Length);
    }
}