using NSubstitute;
using Tgs.Agent.Core;
using Tgs.Agent.Core.ApplicationServices;
using Tgs.Agent.Core.Models;
using Tgs.Agent.Core.Models.Enums;

namespace Agent.Core.UnitTests;

public class SetAgentInfoTests
{
    [Test]
    public async Task Set_Agent_Info()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();

        var agentId = Guid.Parse("8480c811-879b-41e0-8b8b-bd0c9cdbdf31");
        var parentAgentId = Guid.NewGuid();

        var sut = new SetAgentInfoCommandService(fakeAgentRepository);

        var command = new SetAgentInfoCommand(
            agentId,
            parentAgentId,
            AgentStatus.Enabled,
            AgentLevelType.Agent,
            AgentConfigStatus.WaitForConfigurePercentage,
            true,
            "",
            null,
            "editor");

        await sut.ExecuteAsync(command);

        _ = fakeAgentRepository
            .Received(1)
            .SetAgentInfoAsync(Arg.Any<UpdateAgentModel>());
    }
}