using System.Collections.Immutable;
using System.Text;
using System.Text.Json;
using AutoFixture.NUnit3;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;
using Tgs.Agent.Client;
using Tgs.Agent.Client.Models;
using Tgs.Agent.Core;
using Tgs.Agent.Core.Models;
using Tgs.Agent.Repositories.Entities;
using Tgs.Core.Infrastructures.Security;
using Tgs.SerialNumberGenerator;

namespace Tgs.Agent.IntegrationTest;

public class AgentTests
{
    [Test]
    public async Task Get_Agent_Info()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();
        await using var web = TestSiteHelper.GetAgentServiceTestHost(services =>
            services.AddSingleton(fakeAgentRepository));

        var httpClient = web.CreateClient();

        var sut = new AgentClient(httpClient);

        var agentId = Guid.Parse("30945e96-a762-4b6e-9acb-0266727528d7");

        _ = fakeAgentRepository
            .GetAgentByIdAsync(agentId, Arg.Any<CancellationToken>())
            .Returns(new AgentInfo(
                agentId,
                Username: "agent1",
                RealName: null,
                Phone: null,
                Email: null,
                QQ: null,
                Wechat: null,
                InvitationCode: "wg5g8h",
                ParentAgentId: Guid.Parse("8480c811-879b-41e0-8b8b-bd0c9cdbdf31"),
                RegisteredOn: DateTimeOffset.UtcNow,
                Status: Core.AgentStatus.Enabled,
                Type: Core.AgentType.Cash,
                AgentLevelType: Core.AgentLevelType.Chief,
                DividendPercentage: 80M,
                PlayerNumber: 0,
                AccumulatedProfitSharing: 0M,
                BrandId: Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af")));

        var actual = await sut.GetAgentInfoAsync(new(agentId));

        Assert.NotNull(actual);
        Assert.AreEqual(agentId, actual!.Id);
    }

    [Test]
    [AutoData]
    public async Task Get_Agent_Info_By_Username(
        AgentInfo agentInfo,
        GetAgentInfoByUsernameRequest request)
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();
        await using var web = TestSiteHelper.GetAgentServiceTestHost(services =>
            services.AddSingleton(fakeAgentRepository));

        var httpClient = web.CreateClient();

        var sut = new AgentClient(httpClient);

        _ = fakeAgentRepository
            .GetAgentByUsernameAsync(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(agentInfo);

        var actual = await sut.GetAgentInfoByUsernameAsync(request);

        Assert.NotNull(actual);
        Assert.AreEqual(agentInfo.Id, actual!.Id);
    }

    [Test]
    public async Task Get_Child_Agent_Infos()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();
        await using var web = TestSiteHelper.GetAgentServiceTestHost(services =>
            services.AddSingleton(fakeAgentRepository));

        var httpClient = web.CreateClient();

        var sut = new AgentClient(httpClient);

        var parentAgentId = Guid.Parse("8480c811-879b-41e0-8b8b-bd0c9cdbdf31");

        _ = fakeAgentRepository
            .GetChildAgentInfosAsync(parentAgentId, Arg.Any<CancellationToken>())
            .Returns(new Core.Models.ChildAgentInfoModel[]
            {
                new Core.Models.ChildAgentInfoModel
                {
                    AgentId = Guid.Parse("30945e96-a762-4b6e-9acb-0266727528d7"),
                    AgentUsername = "agent1",
                    ParentAgentId = parentAgentId
                },
                new Core.Models.ChildAgentInfoModel
                {
                    AgentId = Guid.Parse("16bf8af5-52a6-404c-924d-3292cb4bf6ef"),
                    AgentUsername = "agent2",
                    ParentAgentId = parentAgentId
                }
            }.ToAsyncEnumerable());

        var actual = await sut.GetChildAgentInfosAsync(new(parentAgentId)).ToArrayAsync();

        Assert.AreEqual(2, actual.Length);
        Assert.AreEqual(parentAgentId, actual[0].ParentAgentId);
    }

    [Test]
    public async Task Set_Agent_Info()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();
        await using var web = TestSiteHelper.GetAgentServiceTestHost(services
            => services.AddSingleton(fakeAgentRepository));

        var httpClient = web.CreateClient();

        var sut = new AgentClient(httpClient);

        var agentId = Guid.Parse("8480c811-879b-41e0-8b8b-bd0c9cdbdf31");

        await sut.SetAgentInfoAsync(new(agentId, "realName"));
    }

    [Test]
    public async Task Set_Agent_Contact()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();
        await using var web = TestSiteHelper.GetAgentServiceTestHost(services
            => services.AddSingleton(fakeAgentRepository));

        var httpClient = web.CreateClient();

        var sut = new AgentClient(httpClient);

        var agentId = Guid.Parse("8480c811-879b-41e0-8b8b-bd0c9cdbdf31");

        await sut.SetAgentContactAsync(new(agentId, "email", "phone"));
    }

    [Test]
    public async Task Set_Agent_Dividend_Percentage()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();
        await using var web = TestSiteHelper.GetAgentServiceTestHost(services
            => services.AddSingleton(fakeAgentRepository));

        var httpClient = web.CreateClient();

        var sut = new AgentClient(httpClient);

        var agentId = Guid.Parse("8480c811-879b-41e0-8b8b-bd0c9cdbdf31");

        await sut.SetAgentDividendPercentageAsync(new(agentId, 68M));
    }

    [Test]
    public async Task Set_Agent_Password()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();
        var fakePasswordHandler = Substitute.For<IPasswordHandler>();
        var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();

        await using var web = TestSiteHelper.GetAgentServiceTestHost(services
            => services
                .AddSingleton(fakeAgentRepository)
                .AddSingleton(fakePasswordHandler)
                .AddSingleton(fakeSerialNumberGenerator));

        var httpClient = web.CreateClient();

        var sut = new AgentClient(httpClient);

        var agentId = Guid.Parse("8480c811-879b-41e0-8b8b-bd0c9cdbdf31");
        var password = "password";
        var modifyUserId = "50ce61eb-8c9d-4f84-3cf6-08d9262ce1a5";
        var brandId = Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af");
        var eventId = Guid.NewGuid();
        var salt = Encoding.UTF8.GetBytes("salt");
        var hashedPassword = Encoding.UTF8.GetBytes("password");

        _ = fakePasswordHandler.GenerateCredential(password)
            .Returns((salt, hashedPassword));

        _ = fakeSerialNumberGenerator
            .GenerateAsync(Arg.Any<CancellationToken>())
            .Returns(eventId);

        _ = fakeAgentRepository.GetAgentByIdAsync(agentId, Arg.Any<CancellationToken>())
            .Returns(new AgentInfo(
            Id: agentId,
            Username: "agentUsername",
            null,
            null,
            null,
            null,
            null,
            "inviteCode",
            null,
            DateTimeOffset.UtcNow.AddMonths(-1),
            Core.AgentStatus.Enabled,
            Core.AgentType.Cash,
            Core.AgentLevelType.Chief,
            0m,
            0,
            0m,
            brandId));

        await sut.SetAgentPasswordAsync(new(agentId, password, modifyUserId));

        _ = fakeAgentRepository
            .Received(1)
            .SetPasswordAsync(Arg.Is(agentId), Arg.Is(hashedPassword), Arg.Is(salt), Arg.Any<CancellationToken>());

        _ = fakeAgentRepository
            .Received(1)
            .CreateAgentEventAsync(Arg.Is(eventId), Arg.Is(brandId), Arg.Is(agentId), Arg.Is(EventAction.Update), Arg.Any<JsonDocument>(), Arg.Is(modifyUserId), Arg.Any<CancellationToken>());

        _ = fakeAgentRepository.Received(1)
            .UpdateAgentSnapshotAsync(Arg.Is(agentId), Arg.Is(eventId), Arg.Any<DateTime>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task 設置代理提款密碼()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();
        await using var web = TestSiteHelper.GetAgentServiceTestHost(services
            => services.AddSingleton(fakeAgentRepository));

        var httpClient = web.CreateClient();

        var sut = new AgentClient(httpClient);

        var agentId = Guid.Parse("8480c811-879b-41e0-8b8b-bd0c9cdbdf31");

        await sut.SetAgentPayPasswordAsync(new(agentId, "password"));
    }

    [Test]
    public async Task 設置代理屬性()
    {
        var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
        await using var web = TestSiteHelper.GetAgentServiceTestHost(services
            => services.AddSingleton(fakeSerialNumberGenerator));

        var httpClient = web.CreateClient();

        var sut = new AgentClient(httpClient);

        var brandId = Guid.NewGuid();
        var agentId = Guid.Parse("8480c811-879b-41e0-8b8b-bd0c9cdbdf31");
        var modifyUserId = "50ce61eb-8c9d-4f84-3cf6-08d9262ce1a5";

        var dbContext = web.Services.GetRequiredService<TgsContext>();

        _ = await dbContext.Agents.AddAsync(new Repositories.Entities.Agent
        {
            Id = agentId,
            BrandId = brandId,
            Password = Array.Empty<byte>(),
            Salt = Array.Empty<byte>(),
            Username = "agentUsername",
            RealName = "agentRealName",
            WithdrawPassword = Array.Empty<byte>(),
            WithdrawSalt = Array.Empty<byte>(),
            InvitationCode = "hgq0m0",
            RegisteredOn = DateTime.UtcNow.AddMonths(-1),
        });

        _ = await dbContext.SaveChangesAsync();

        var agentProperties = new Dictionary<Client.AgentPropertyName, string?>
        {
            [Client.AgentPropertyName.RealName] = null,
            [Client.AgentPropertyName.Qq] = "NewQq",
            [Client.AgentPropertyName.Wechat] = "NewWechat"
        };

        await sut.SetAgentPropertiesAsync(agentId, agentProperties.ToImmutableDictionary(), modifyUserId);

        var actual = await dbContext.Agents
            .Where(x => x.Id == agentId)
            .FirstAsync();

        Assert.IsNull(actual.RealName);
        Assert.AreEqual("NewQq", actual.Qq);
        Assert.AreEqual("NewWechat", actual.Wechat);
    }

    [Test]
    public async Task 設置代理狀態()
    {
        var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
        var fakeSystemClock = Substitute.For<ISystemClock>();
        await using var web = TestSiteHelper.GetAgentServiceTestHost(services
            => services
                .AddSingleton(fakeSerialNumberGenerator)
                .AddSingleton(fakeSystemClock));

        var httpClient = web.CreateClient();

        var sut = new AgentClient(httpClient);

        var brandId = Guid.NewGuid();
        var eventId = Guid.NewGuid();
        var agentId = Guid.Parse("8480c811-879b-41e0-8b8b-bd0c9cdbdf31");
        var status = Client.Models.Enums.AgentStatus.InBlacklist;
        var modifyUserId = "50ce61eb-8c9d-4f84-3cf6-08d9262ce1a5";
        var now = DateTimeOffset.UtcNow;

        var dbContext = web.Services.GetRequiredService<TgsContext>();

        var agent = new Repositories.Entities.Agent
        {
            Id = agentId,
            Status = (short)AgentStatus.Enabled,
            BrandId = brandId,
            Password = Array.Empty<byte>(),
            Salt = Array.Empty<byte>(),
            Username = "agentUsername",
            RealName = "agentRealName",
            WithdrawPassword = Array.Empty<byte>(),
            WithdrawSalt = Array.Empty<byte>(),
            InvitationCode = "hgq0m0",
            RegisteredOn = DateTime.UtcNow.AddMonths(-1),
        };

        _ = await dbContext.Agents.AddAsync(agent);

        _ = await dbContext.SaveChangesAsync();

        _ = fakeSerialNumberGenerator
            .GenerateAsync(Arg.Any<CancellationToken>())
            .Returns(eventId);

        _ = fakeSystemClock.UtcNow.Returns(now);

        await sut.SetAgentStatusAsync(agentId, status, modifyUserId);

        Assert.AreEqual((short)Client.Models.Enums.AgentStatus.InBlacklist, agent.Status);
    }

    [Test]
    public async Task 查詢代理歷史紀錄()
    {
        var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
        var fakeSystemClock = Substitute.For<ISystemClock>();
        await using var web = TestSiteHelper.GetAgentServiceTestHost(services
            => services
                .AddSingleton(fakeSerialNumberGenerator)
                .AddSingleton(fakeSystemClock));

        var httpClient = web.CreateClient();

        var sut = new AgentClient(httpClient);

        var brandId = Guid.NewGuid();
        var eventId = Guid.NewGuid();
        var agentId = Guid.Parse("8480c811-879b-41e0-8b8b-bd0c9cdbdf31");

        var dbContext = web.Services.GetRequiredService<TgsContext>();

        var agentEvent = new AgentEventSource
        {
            Id = eventId,
            AgentId = agentId,
            BrandId = brandId,
            ChangeSets = "{}",
            Action = EventAction.Create.ToString(),
            CreatedOn = DateTime.UtcNow.AddDays(-1),
        };

        _ = await dbContext.AgentEventSources.AddAsync(agentEvent);

        _ = await dbContext.SaveChangesAsync();

        var conditions = new GetAgentHistoriesQueryConditions(agentId, DateTime.Now.AddDays(-14), DateTime.Now);

        var actual = await sut.GetAgentHistoriesAsync(conditions, 10, 0).ToArrayAsync();

        Assert.AreEqual(1, actual.Length);
        Assert.AreEqual(Client.Models.Enums.EventAction.Create, actual[0].Action);
        Assert.AreEqual(eventId, actual[0].EventId);
    }

    [Test]
    public async Task 查詢代理歷史紀錄筆數()
    {
        var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
        var fakeSystemClock = Substitute.For<ISystemClock>();
        await using var web = TestSiteHelper.GetAgentServiceTestHost(services
            => services
                .AddSingleton(fakeSerialNumberGenerator)
                .AddSingleton(fakeSystemClock));

        var httpClient = web.CreateClient();

        var sut = new AgentClient(httpClient);

        var brandId = Guid.NewGuid();
        var eventId = Guid.NewGuid();
        var agentId = Guid.Parse("8480c811-879b-41e0-8b8b-bd0c9cdbdf31");

        var dbContext = web.Services.GetRequiredService<TgsContext>();

        var agentEvent = new AgentEventSource
        {
            Id = eventId,
            AgentId = agentId,
            BrandId = brandId,
            ChangeSets = "{}",
            Action = EventAction.Create.ToString(),
            CreatedOn = DateTime.UtcNow.AddDays(-1),
        };

        _ = await dbContext.AgentEventSources.AddAsync(agentEvent);

        _ = await dbContext.SaveChangesAsync();

        var conditions = new GetAgentHistoriesQueryConditions(agentId, DateTime.Now.AddDays(-14), DateTime.Now);

        var actual = await sut.CountAgentHistoriesAsync(conditions);

        Assert.AreEqual(1, actual);
    }

    [Test]
    public async Task 查詢代理歷史紀錄細項()
    {
        var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
        var fakeSystemClock = Substitute.For<ISystemClock>();
        await using var web = TestSiteHelper.GetAgentServiceTestHost(services
            => services
                .AddSingleton(fakeSerialNumberGenerator)
                .AddSingleton(fakeSystemClock));

        var httpClient = web.CreateClient();

        var sut = new AgentClient(httpClient);

        var brandId = Guid.NewGuid();
        var eventId = Guid.NewGuid();
        var agentId = Guid.Parse("8480c811-879b-41e0-8b8b-bd0c9cdbdf31");

        var dbContext = web.Services.GetRequiredService<TgsContext>();

        var agentEvent = new AgentEventSource
        {
            Id = eventId,
            AgentId = agentId,
            BrandId = brandId,
            ChangeSets = "{}",
            Action = EventAction.Create.ToString(),
            CreatedOn = DateTime.UtcNow.AddDays(-1),
        };

        _ = await dbContext.AgentEventSources.AddAsync(agentEvent);

        _ = await dbContext.SaveChangesAsync();

        var actual = await sut.GetAgentHistoryAsync(eventId);

        Assert.NotNull(actual);
        Assert.AreEqual(Client.Models.Enums.EventAction.Create, actual!.Action);
    }

    [Test]
    public async Task 更新子代理們path()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();

        await using var web = TestSiteHelper.GetAgentServiceTestHost(services
            => services
                .AddSingleton(fakeAgentRepository));

        var httpClient = web.CreateClient();

        var agentId = Guid.Parse("09d7371b-556e-4d86-bac4-bba727e8dedd");
        var agentNLevel = 2;
        var agentPath = "c3d9bf119ea24d1c84dc2ed9b8a55fdd.09d7371b556e4d86bac4bba727e8dedd";
        var newParentAgentId = Guid.Parse("c3d9bf11-9ea2-4d1c-84dc-2ed9b8a55fdd");
        var newParentPath = "c3d9bf119ea24d1c84dc2ed9b8a55fdd";

        _ = fakeAgentRepository.GetAgentPathInfoAsync(Arg.Is(agentId), Arg.Any<CancellationToken>())
            .Returns(new AgentPathInfo(agentId, agentNLevel, agentPath));

        _ = fakeAgentRepository.GetAgentPathInfoAsync(Arg.Is(newParentAgentId), Arg.Any<CancellationToken>())
            .Returns(new AgentPathInfo(newParentAgentId, 1, newParentPath));

        var sut = new AgentClient(httpClient);

        await sut.UpdateSubAgentsPathAsync(agentId, newParentAgentId);

        _ = fakeAgentRepository
            .Received(1)
            .UpdateSubAgentsPathAsync(Arg.Is(agentId), Arg.Is(agentNLevel), Arg.Is(agentPath), Arg.Is(newParentPath), Arg.Any<CancellationToken>());
    }
}