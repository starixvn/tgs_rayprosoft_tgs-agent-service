using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.Agent;
using Tgs.Agent.ApplicationServices;
using Tgs.Agent.Models;

namespace Agent.Core.UnitTests;

public class SetAgentInfoTests
{
    [Test]
    public async Task Set_Agent_Info()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();

        var agentId = Guid.Parse("8480c811-879b-41e0-8b8b-bd0c9cdbdf31");
        var subAgentId = Guid.Parse("30945e96-a762-4b6e-9acb-0266727528d7");

        _ = fakeAgentRepository
            .GetAgentByIdAsync(subAgentId)
            .Returns(new AgentInfo(subAgentId, ParentAgentId: agentId, null, null, null));

        var sut = new SetAgentInfoCommandService(fakeAgentRepository, NullLogger<SetAgentInfoCommandService>.Instance);

        var command = new SetAgentInfoCommand(agentId, subAgentId, "realName", "email", "phone");

        await sut.ExecuteAsync(command);

        _ = fakeAgentRepository
            .Received(1)
            .SetAgentInfoAsync(Arg.Is(command.SubAgentId), Arg.Is(command.RealName), Arg.Is(command.Email), Arg.Is(command.Phone));
    }
}