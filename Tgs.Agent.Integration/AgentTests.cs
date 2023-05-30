using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;
using Tgs.Agent.Client;
using Tgs.Agent.Core.Models;

namespace Tgs.Agent.IntegrationTest
{
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
                    Status: 1,
                    Type: 1,
                    AgentLevelType: 1,
                    DividendPercentage: 80M,
                    PlayerNumber: 0,
                    AccumulatedProfitSharing: 0M));

            var actual = await sut.GetAgentInfoAsync(new(agentId));

            Assert.NotNull(actual);
            Assert.AreEqual(agentId, actual!.Id);
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

            await sut.SetAgentInfoAsync(new(agentId, "realName", "email", "phone"));
        }

        [Test]
        public async Task Set_Agent_Password()
        {
            var fakeAgentRepository = Substitute.For<IAgentRepository>();
            await using var web = TestSiteHelper.GetAgentServiceTestHost(services
                => services.AddSingleton(fakeAgentRepository));

            var httpClient = web.CreateClient();

            var sut = new AgentClient(httpClient);

            var agentId = Guid.Parse("8480c811-879b-41e0-8b8b-bd0c9cdbdf31");

            await sut.SetAgentPasswordAsync(new(agentId, "password"));
        }
    }
}