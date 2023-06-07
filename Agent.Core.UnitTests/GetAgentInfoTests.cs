using AutoFixture;
using NSubstitute;
using Tgs.Agent.Core;
using Tgs.Agent.Core.ApplicationServices;
using Tgs.Agent.Core.Models;

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

    [Test]
    public async Task Get_Agent_Info_By_Username()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();

        var sut = new GetAgentInfoByUsernameQueryService(fakeAgentRepository);

        var query = new GetAgentInfoByUsernameQuery(Guid.NewGuid(), "username");

        var fixture = new Fixture();

        var agentInfo = fixture.Create<AgentInfo>();

        fakeAgentRepository.GetAgentByUsernameAsync(Arg.Any<Guid>(),Arg.Any<string>()).Returns(agentInfo);

        var response = await sut.GetAsync(query);

        Assert.AreEqual(agentInfo, response!);
    }
}