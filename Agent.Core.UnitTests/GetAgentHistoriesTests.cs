using NSubstitute;
using Tgs.Agent.Core;
using Tgs.Agent.Core.ApplicationServices;

namespace Agent.Core.UnitTests;

public class GetAgentHistoriesTests
{
    [Test]
    public async Task 查詢代理歷史紀錄()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();

        var sut = new GetAgentHistoriesQueryService(fakeAgentRepository);
        var agentId = Guid.NewGuid();
        var from = DateTime.Now.AddDays(-14);
        var to = DateTime.Now;

        _ = await sut.QueryAsync(new GetAgentHistoriesQuery(agentId, from, to)).ToArrayAsync();

        _ = fakeAgentRepository.Received(1)
            .QueryAgentEventAsync(Arg.Is(new AgentEventQueryConditions(agentId, from, to)));
    }

    [Test]
    public async Task 查詢代理歷史紀錄筆數()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();

        var sut = new GetAgentHistoriesQueryService(fakeAgentRepository);
        var agentId = Guid.NewGuid();
        var from = DateTime.Now.AddDays(-14);
        var to = DateTime.Now;

        _ = await sut.GetAsync(new GetAgentHistoriesQuery(agentId, from, to));

        _ = fakeAgentRepository.Received(1)
            .CountAgentEventAsync(Arg.Is(new AgentEventQueryConditions(agentId, from, to)));
    }

    [Test]
    public async Task 查詢代理歷史紀錄細項()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();

        var sut = new GetAgentEventQueryService(fakeAgentRepository);

        var eventId = Guid.NewGuid();

        _ = await sut.GetAsync(new GetAgentEventQuery(eventId));

        _ = fakeAgentRepository.Received(1)
            .GetAgentEventAsync(Arg.Is(eventId));
    }
}