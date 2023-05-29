using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;
using Tgs.Agent.Client;
using Tgs.Agent.Models;

namespace Tgs.Agent.IntegrationTest
{
    public class SubAgentTests
    {
        [Test]
        public async Task Reset_SubAgent_Password()
        {
            var fakeAgentRepository = Substitute.For<IAgentRepository>();
            await using var web = TestSiteHelper.GetAgentServiceTestHost(services
                => services.AddSingleton(fakeAgentRepository));

            var httpClient = web.CreateClient();

            var sut = new AgentClient(httpClient);

            var agentId = Guid.Parse("8480c811-879b-41e0-8b8b-bd0c9cdbdf31");
            var subAgentId = Guid.Parse("30945e96-a762-4b6e-9acb-0266727528d7");

            _ = fakeAgentRepository
                .GetAgentByIdAsync(Arg.Is(subAgentId), Arg.Any<CancellationToken>())
                .Returns(new AgentInfo(subAgentId, agentId, null, null, null));

            await sut.ResetSubAgentPasswordAsync(new(agentId, subAgentId, "password"));
        }

        [Test]
        public async Task Set_SubAgent_Info()
        {
            var fakeAgentRepository = Substitute.For<IAgentRepository>();
            await using var web = TestSiteHelper.GetAgentServiceTestHost(services
                => services.AddSingleton(fakeAgentRepository));

            var httpClient = web.CreateClient();

            var sut = new AgentClient(httpClient);

            var agentId = Guid.Parse("8480c811-879b-41e0-8b8b-bd0c9cdbdf31");
            var subAgentId = Guid.Parse("30945e96-a762-4b6e-9acb-0266727528d7");

            _ = fakeAgentRepository
                .GetAgentByIdAsync(Arg.Is(subAgentId), Arg.Any<CancellationToken>())
                .Returns(new AgentInfo(subAgentId, agentId, "abc", "123", "abc"));

            await sut.SetSubAgentInfoAsync(new(agentId, subAgentId, "realName", "email", "phone"));
        }
    }
}