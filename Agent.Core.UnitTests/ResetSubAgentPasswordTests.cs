using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.Agent;
using Tgs.Agent.Core.ApplicationServices;
using Tgs.Agent.Models;
using Tgs.Core.Infrastructures.Security;

namespace Agent.Core.UnitTests;

public class ResetSubAgentPasswordTests
{
    [Test]
    public async Task Not_Found_Sub_Agent()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();
        var fakePasswordHandler = Substitute.For<IPasswordHandler>();

        var newPassword = "newPassword";

        var sut = new ResetSubAgentPasswordCommandService(
            fakeAgentRepository,
            fakePasswordHandler,
            NullLogger<ResetSubAgentPasswordCommandService>.Instance);

        await sut.ExecuteAsync(new ResetSubAgentPasswordCommand(
            Guid.Parse("8480c811-879b-41e0-8b8b-bd0c9cdbdf31"),
            Guid.Parse("30945e96-a762-4b6e-9acb-0266727528d7"),
            newPassword));

        _ = fakePasswordHandler
            .Received(0)
            .GenerateCredential(Arg.Is(newPassword));
    }

    [Test]
    public async Task 非直屬代理不能修改代理密碼()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();
        var fakePasswordHandler = Substitute.For<IPasswordHandler>();

        var agentId = Guid.Parse("8480c811-879b-41e0-8b8b-bd0c9cdbdf31");
        var subAgentId = Guid.Parse("30945e96-a762-4b6e-9acb-0266727528d7");

        _ = fakeAgentRepository
            .GetAgentByIdAsync(subAgentId)
            .Returns(new AgentInfo(
                subAgentId,
                ParentAgentId: Guid.Parse("16bf8af5-52a6-404c-924d-3292cb4bf6ef"),
                "realName",
                "phone",
                "email"));

        var newPassword = "newPassword";

        var sut = new ResetSubAgentPasswordCommandService(
            fakeAgentRepository,
            fakePasswordHandler,
            NullLogger<ResetSubAgentPasswordCommandService>.Instance);

        await sut.ExecuteAsync(new ResetSubAgentPasswordCommand(agentId, Guid.Parse("30945e96-a762-4b6e-9acb-0266727528d7"), newPassword));

        _ = fakePasswordHandler
            .Received(0)
            .GenerateCredential(Arg.Is(newPassword));
    }

    [Test]
    public async Task Reset_Sub_Agent_Password()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();
        var fakePasswordHandler = Substitute.For<IPasswordHandler>();

        var agentId = Guid.Parse("8480c811-879b-41e0-8b8b-bd0c9cdbdf31");
        var subAgentId = Guid.Parse("30945e96-a762-4b6e-9acb-0266727528d7");

        _ = fakeAgentRepository
            .GetAgentByIdAsync(subAgentId)
            .Returns(new AgentInfo(subAgentId, ParentAgentId: agentId, null, null, null));

        var newPassword = "newPassword";
        var credential = (Encoding.UTF8.GetBytes("salt"), Encoding.UTF8.GetBytes("hashedPassword"));

        _ = fakePasswordHandler
            .GenerateCredential(newPassword)
            .Returns(credential);

        var sut = new ResetSubAgentPasswordCommandService(
            fakeAgentRepository,
            fakePasswordHandler,
            NullLogger<ResetSubAgentPasswordCommandService>.Instance);

        await sut.ExecuteAsync(new ResetSubAgentPasswordCommand(agentId, subAgentId, newPassword));

        _ = fakeAgentRepository
            .Received(1)
            .SetPasswordAsync(Arg.Is(subAgentId), Arg.Is(credential.Item2), Arg.Is(credential.Item1));
    }
}