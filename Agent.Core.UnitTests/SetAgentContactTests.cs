using NSubstitute;
using Tgs.Agent.Core;
using Tgs.Agent.Core.ApplicationServices;

namespace Agent.Core.UnitTests;

public class SetAgentContactTests
{
    [Test]
    public async Task Set_Agent_Contact()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();

        var agentId = Guid.Parse("8480c811-879b-41e0-8b8b-bd0c9cdbdf31");

        var sut = new SetAgentContactCommandService(fakeAgentRepository);

        var command = new SetAgentContactCommand(
            agentId,
            "email",
            "phone");

        await sut.ExecuteAsync(command);

        _ = fakeAgentRepository
            .Received(1)
            .SetAgentContactAsync(
                Arg.Is(command.AgentId),
                Arg.Is(command.Email),
                Arg.Is(command.Phone));
    }
}