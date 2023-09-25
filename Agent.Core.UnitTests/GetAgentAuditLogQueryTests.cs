using NSubstitute;
using Tgs.Agent.Core;
using Tgs.Agent.Core.ApplicationServices;

namespace Agent.Core.UnitTests;

public class GetAgentAuditLogQueryTests
{
    [Test]
    public async Task 查詢代理操作紀錄()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();

        var sut = new GetAgentAuditLogQueryService(fakeAgentRepository);

        var brandId = Guid.NewGuid();
        var agentId = Guid.NewGuid();
        var from = DateTimeOffset.Now.AddDays(-14);
        var to = DateTimeOffset.Now;

        var paging = new Tgs.Paging.Paging<AgentAuditLogInfo>
        {
            IsDesc = false,
            Page = 1,
            RowCount = 20,
        };

        _ = await sut.QueryAsync(new GetAgentAuditLogQuery(brandId, agentId, from, to, paging)).ToArrayAsync();

        _ = fakeAgentRepository
            .Received(1)
            .QueryAgentAuditLogsAsync(Arg.Is(new AgenAuditLogQueryConditions(brandId, agentId, from, to)), Arg.Is(paging));
    }

    [Test]
    public async Task 查詢代理操作紀錄總筆數()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();

        var sut = new GetAgentAuditLogQueryService(fakeAgentRepository);

        var brandId = Guid.NewGuid();
        var agentId = Guid.NewGuid();
        var from = DateTimeOffset.Now.AddDays(-14);
        var to = DateTimeOffset.Now;

        var paging = new Tgs.Paging.Paging<AgentAuditLogInfo>
        {
            IsDesc = false,
            Page = 1,
            RowCount = 20,
        };

        _ = await sut.GetAsync(new GetAgentAuditLogQuery(brandId, agentId, from, to, paging));

        _ = fakeAgentRepository
            .Received(1)
            .CountAgentAuditLogsAsync(Arg.Is(new AgenAuditLogQueryConditions(brandId, agentId, from, to)));
    }
}