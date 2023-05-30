using NSubstitute;
using Tgs.Agent;
using Tgs.Agent.Core.ApplicationServices;

namespace Agent.Core.UnitTests;

public class SetAgentInfoTests
{
    [Test]
    public async Task Set_Agent_Info()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();

        var agentId = Guid.Parse("8480c811-879b-41e0-8b8b-bd0c9cdbdf31");

        var sut = new SetAgentInfoCommandService(fakeAgentRepository);

        var command = new SetAgentInfoCommand(
            agentId,
            "realName",
            "email",
            "phone");

        await sut.ExecuteAsync(command);

        _ = fakeAgentRepository
            .Received(1)
            .SetAgentInfoAsync(
                Arg.Is(command.AgentId),
                Arg.Is(command.RealName),
                Arg.Is(command.Email),
                Arg.Is(command.Phone));
    }
}