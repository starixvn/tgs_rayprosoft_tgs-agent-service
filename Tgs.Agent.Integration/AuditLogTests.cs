using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;
using Tgs.Agent.Client;
using Tgs.Agent.Client.Models.Enums;
using Tgs.Agent.Core;
using Tgs.Paging;

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

            _ = fakeAgentRepository
                .Received(1)
                .AddAgentAuditLogAsync(agentId, Core.Models.Enums.AgentOperation.Info, "editUsername", Arg.Any<CancellationToken>());
        }

        [Test]
        public async Task 查詢代理操作紀錄()
        {
            var fakeAgentRepository = Substitute.For<IAgentRepository>();
            await using var web = TestSiteHelper.GetAgentServiceTestHost(services =>
                services.AddSingleton(fakeAgentRepository));

            var httpClient = web.CreateClient();

            var sut = new AgentClient(httpClient);

            var agentId = Guid.Parse("30945e96-a762-4b6e-9acb-0266727528d7");
            var brandId = Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af");
            var from = DateTimeOffset.Now.AddDays(-14);
            var to = DateTimeOffset.Now;

            var conditions = new AgenAuditLogQueryConditions(brandId, agentId, from, to);

            _ = fakeAgentRepository
                .QueryAgentAuditLogsAsync(
                    Arg.Is(conditions),
                    Arg.Any<Paging<AgentAuditLogInfo>>(),
                    Arg.Any<CancellationToken>())
                .Returns(new AgentAuditLogInfo[]
                {
                    new AgentAuditLogInfo(
                        Guid.NewGuid(),
                        Core.Models.Enums.AgentOperation.PayPassword,
                        "editUsername",
                        DateTimeOffset.Now.AddDays(-1))
                }.ToAsyncEnumerable());

            _ = fakeAgentRepository
                .CountAgentAuditLogsAsync(
                    Arg.Is(conditions), 
                    Arg.Any<CancellationToken>())
                .Returns(1);

            var queryConditions = new Client.Models.GetAgentAuditLogQueryConditions
            {
                AgentId = agentId,
                BrandId = brandId,
                From = from,
                To = to
            };

            var paging = new Client.Models.Paging
            {
                IsDesc = false,
                Page = 1,
                Rows = 20,
            };

            var actual = await sut.GetAgentAuditLogsAsync(queryConditions, paging);

            Assert.AreEqual(1, actual.TotalCount);
            Assert.AreEqual(AgentOperation.PayPassword, actual.Items.First().Operation);
        }
    }
}