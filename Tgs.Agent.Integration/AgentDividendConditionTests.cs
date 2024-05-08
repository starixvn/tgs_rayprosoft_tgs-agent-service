using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;
using Tgs.Agent.Client;
using Tgs.Agent.Core;

namespace Tgs.Agent.IntegrationTest;

public class AgentDividendConditionTests
{
    [Test]
    public async Task 清除代理階梯式分紅設置()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();

        await using var web = new AgentTestApplication(builder
            => builder.ConfigureServices(services => services.AddSingleton(fakeAgentRepository)));

        var httpClient = web.CreateClient();

        var sut = new AgentClient(httpClient);

        var agentId = Guid.Parse("8480c811-879b-41e0-8b8b-bd0c9cdbdf31");

        await sut.ClearAgentDividendConditionsAsync(agentId);

        _ = fakeAgentRepository
            .Received(1)
            .ClearAgentDividendConditionsAsync(Arg.Is(agentId), Arg.Any<CancellationToken>());
    }
}