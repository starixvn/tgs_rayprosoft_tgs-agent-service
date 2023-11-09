using System.Text.Json;
using NSubstitute;
using Tgs.Agent.Core;
using Tgs.Agent.Core.ApplicationServices;
using Tgs.Agent.Core.Models;
using Tgs.SerialNumberGenerator;

namespace Agent.Core.UnitTests;

public class SetAgentStatusTests
{
    [Test]
    public async Task 變更代理狀態()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();
        var fakeSystemClock = Substitute.For<ISystemClock>();
        var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();

        var sut = new SetAgentStatusCommandService(fakeAgentRepository, fakeSystemClock, fakeSerialNumberGenerator);

        var brandId = Guid.NewGuid();
        var agentId = Guid.Parse("8480c811-879b-41e0-8b8b-bd0c9cdbdf31");
        var status = AgentStatus.Enabled;
        var modifyUserId = "50ce61eb-8c9d-4f84-3cf6-08d9262ce1a5";
        var eventId = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;

        _ = fakeSerialNumberGenerator
            .GenerateAsync()
            .Returns(eventId);

        _ = fakeSystemClock.UtcNow
            .Returns(now);

        var agentInfo = new AgentInfo(
            agentId,
            "agentUsername",
            null,
            null,
            null,
            null,
            null,
            "inviteCode",
            null,
            DateTimeOffset.Now.AddMonths(-1),
            AgentStatus.Enabled,
            AgentType.Cash,
            AgentLevelType.Chief,
            0m,
            0,
            0m,
            brandId,
            Guid.NewGuid(),
            null);

        _ = fakeAgentRepository
            .GetAgentByIdAsync(Arg.Is(agentId))
            .Returns(agentInfo);

        await sut.ExecuteAsync(new SetAgentStatusCommand(agentId, status, modifyUserId));

        _ = fakeAgentRepository
            .Received(1)
            .SetAgentStatusAsync(Arg.Is(agentId), Arg.Is(status));

        _ = fakeAgentRepository
            .Received(1)
            .CreateAgentEventAsync(
                Arg.Is(eventId),
                Arg.Is(brandId),
                Arg.Is(agentId),
                Arg.Is(EventAction.Update),
                Arg.Any<JsonDocument>(),
                Arg.Is(modifyUserId));

        _ = fakeAgentRepository
            .Received(1)
            .UpdateAgentSnapshotAsync(
                Arg.Is(agentId),
                Arg.Is(eventId),
                Arg.Is(now.DateTime));
    }
}