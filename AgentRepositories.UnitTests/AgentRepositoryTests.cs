using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
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

        var sut = new AgentRepository(dbContext);

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

        var sut = new AgentRepository(dbContext);

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

        var sut = new AgentRepository(dbContext);

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

        var sut = new AgentRepository(dbContext);

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

        var sut = new AgentRepository(dbContext);

        await sut.SetAgentInfoAsync(agentId, realName);

        var actual = await dbContext.Agents.FirstOrDefaultAsync(x => x.Id == agentId);

        Assert.NotNull(actual);
        Assert.AreEqual(actual!.RealName, realName);
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

        var sut = new AgentRepository(dbContext);

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

        var sut = new AgentRepository(dbContext);

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

        var sut = new AgentRepository(dbContext);

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

        var sut = new AgentRepository(dbContext);

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

        var sut = new AgentRepository(dbContext);

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
}