using System.Text;
using NSubstitute;
using Tgs.Agent.Core;
using Tgs.Agent.Core.ApplicationServices;
using Tgs.Core.Infrastructures.Security;

namespace Agent.Core.UnitTests;

public class SetAgentPayPasswordTests
{
    [Test]
    public async Task 更新代理提款密碼()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();
        var fakePasswordHandler = Substitute.For<IPasswordHandler>();

        var agentId = Guid.Parse("8480c811-879b-41e0-8b8b-bd0c9cdbdf31");

        var newPassword = "newPassword";
        var credential = (Encoding.UTF8.GetBytes("salt"), Encoding.UTF8.GetBytes("hashedPassword"));

        _ = fakePasswordHandler
            .GenerateCredential(newPassword)
            .Returns(credential);

        var sut = new SetAgentPayPasswordCommandService(fakeAgentRepository, fakePasswordHandler);
        await sut.ExecuteAsync(new SetAgentPayPasswordCommand(agentId, newPassword));

        _ = fakeAgentRepository
            .Received(1)
            .SetPayPasswordAsync(Arg.Is(agentId), Arg.Is(credential.Item2), Arg.Is(credential.Item1));
    }
}