using NSubstitute;
using Tgs.Agent.Core;
using Tgs.Agent.Core.ApplicationServices;
using Tgs.Agent.Core.Models.Enums;

namespace Agent.Core.UnitTests;

public class ClearNegativeCommissionTests
{
    [Test]
    public async Task Clear_Negative_CommissionTests()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();

        var sut = new ClearNegativeCommissionCommandService(fakeAgentRepository);

        var agentId = Guid.NewGuid();
        var clearByUserId = Guid.NewGuid();

        var command = new ClearNegativeCommissionCommand(agentId, clearByUserId);

        await sut.ExecuteAsync(command);

        _ = fakeAgentRepository
            .Received(1)
            .ClearNegativeCommissionAsync(Arg.Is(command.AgentId), Arg.Is(command.ClearByUserId), Arg.Any<CancellationToken>());
    }
}