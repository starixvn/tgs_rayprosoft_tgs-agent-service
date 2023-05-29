using NSubstitute;
using Tgs.Agent;
using Tgs.Agent.Core.ApplicationServices;

namespace Agent.Core.UnitTests;

public class ChildAgentInfosQueryTests
{
    [Test]
    public async Task Child_Agent_Infos()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();

        var sut = new GetChildAgentInfosQueryService(fakeAgentRepository);

        var query = new GetChildAgentInfosQuery(Guid.NewGuid());

        _ = await sut.QueryAsync(query).ToArrayAsync();

        _ = fakeAgentRepository
            .Received(1)
            .GetChildAgentInfosAsync(Arg.Is(query.AgentId));
    }
}