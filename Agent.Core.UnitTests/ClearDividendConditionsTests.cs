using NSubstitute;
using Tgs.Agent.Core;
using Tgs.Agent.Core.ApplicationServices;

namespace Agent.Core.UnitTests;

public class ClearDividendConditionsTests
{
    [Test]
    public async Task 清除代理階梯式分紅設置()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();

        var sut = new ClearDividendConditionsCommandService(fakeAgentRepository);

        var agentId = Guid.NewGuid();
        await sut.ExecuteAsync(new ClearDividendConditionsCommand(agentId));

        _ = fakeAgentRepository
            .Received(1)
            .ClearAgentDividendConditionsAsync(Arg.Is(agentId));
    }
}