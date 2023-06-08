using System.Text;
using NSubstitute;
using Tgs.Agent.Core;
using Tgs.Agent.Core.ApplicationServices;
using Tgs.Core.Infrastructures.Security;

namespace Agent.Core.UnitTests;

public class SetAgentPasswordTests
{
    [Test]
    public async Task Set_Agent_Password()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();
        var fakePasswordHandler = Substitute.For<IPasswordHandler>();

        var agentId = Guid.Parse("8480c811-879b-41e0-8b8b-bd0c9cdbdf31");

        var newPassword = "newPassword";
        var credential = (Encoding.UTF8.GetBytes("salt"), Encoding.UTF8.GetBytes("hashedPassword"));

        _ = fakePasswordHandler
            .GenerateCredential(newPassword)
            .Returns(credential);

        var sut = new SetAgentPasswordCommandService(
            fakeAgentRepository,
            fakePasswordHandler);

        await sut.ExecuteAsync(new SetAgentPasswordCommand(agentId, newPassword));

        _ = fakeAgentRepository
            .Received(1)
            .SetPasswordAsync(Arg.Is(agentId), Arg.Is(credential.Item2), Arg.Is(credential.Item1));
    }
}