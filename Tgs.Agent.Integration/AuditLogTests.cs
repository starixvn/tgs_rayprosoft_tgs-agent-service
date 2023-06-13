using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;
using Tgs.Agent.Client;
using Tgs.Agent.Client.Models.Enums;
using Tgs.Agent.Core;

namespace Tgs.Agent.IntegrationTest
{
    public class AuditLogTests
    {
        [Test]
        public async Task Add_Agent_Audit_Log()
        {
            var fakeAgentRepository = Substitute.For<IAgentRepository>();
            await using var web = TestSiteHelper.GetAgentServiceTestHost(services =>
                services.AddSingleton(fakeAgentRepository));

            var httpClient = web.CreateClient();

            var sut = new AgentClient(httpClient);

            var agentId = Guid.Parse("30945e96-a762-4b6e-9acb-0266727528d7");

            await sut.AddAgentAuditLogAsync(new(agentId, AgentOperation.Info, "editUsername"));
        }
    }
}