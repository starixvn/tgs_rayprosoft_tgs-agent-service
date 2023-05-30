using NSubstitute;
using Tgs.Agent;
using Tgs.Agent.Core.ApplicationServices;

namespace Agent.Core.UnitTests;

public class GetAgentInfoTests
{
    [Test]
    public async Task Get_Agent_Info()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();

        var sut = new GetAgentInfoQueryService(fakeAgentRepository);

        var query = new GetAgentInfoQuery(Guid.NewGuid());

        _ = await sut.GetAsync(query);

        _ = fakeAgentRepository
            .Received(1)
            .GetAgentByIdAsync(Arg.Is(query.AgentId));
    }
}