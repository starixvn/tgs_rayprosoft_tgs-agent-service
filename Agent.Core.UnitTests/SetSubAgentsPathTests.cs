using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.Agent.Core;
using Tgs.Agent.Core.ApplicationServices;
using Tgs.Agent.Core.Models;

namespace Agent.Core.UnitTests;

public class SetSubAgentsPathTests
{
    [Test]
    public async Task 設置子代理們Path()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();
        var sut = new SetSubAgentsPathCommandService(fakeAgentRepository, NullLogger<SetSubAgentsPathCommandService>.Instance);

        var agentId = Guid.Parse("09d7371b-556e-4d86-bac4-bba727e8dedd");
        var agentNLevel = 2;
        var agentPath = "c3d9bf119ea24d1c84dc2ed9b8a55fdd.09d7371b556e4d86bac4bba727e8dedd";
        var newParentAgentId = Guid.Parse("c3d9bf11-9ea2-4d1c-84dc-2ed9b8a55fdd");
        var newParentPath = "c3d9bf119ea24d1c84dc2ed9b8a55fdd";

        _ = fakeAgentRepository.GetAgentPathInfoAsync(Arg.Is(agentId))
            .Returns(new AgentPathInfo(agentId, agentNLevel, agentPath));

        _ = fakeAgentRepository.GetAgentPathInfoAsync(Arg.Is(newParentAgentId))
            .Returns(new AgentPathInfo(newParentAgentId, 1, newParentPath));

        await sut.ExecuteAsync(new SetSubAgentsPathCommand(agentId, newParentAgentId));

        _ = fakeAgentRepository
            .Received(1)
            .UpdateSubAgentsPathAsnyc(Arg.Is(agentId), Arg.Is(agentNLevel), Arg.Is(agentPath), Arg.Is(newParentPath));
    }
}