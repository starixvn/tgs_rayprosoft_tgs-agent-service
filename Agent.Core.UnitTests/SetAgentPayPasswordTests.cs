using System.Text;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Tgs.Agent.Core;
using Tgs.Agent.Core.ApplicationServices;
using Tgs.Agent.Core.Models;
using Tgs.Core.Infrastructures.Security;
using Tgs.SerialNumberGenerator;

namespace Agent.Core.UnitTests;

public class SetAgentPayPasswordTests
{
    [Test]
    public async Task 更新代理提款密碼()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();
        var fakePasswordHandler = Substitute.For<IPasswordHandler>();
        var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();

        var agentId = Guid.Parse("8480c811-879b-41e0-8b8b-bd0c9cdbdf31");
        var modifyUserId = "50ce61eb-8c9d-4f84-3cf6-08d9262ce1a5";
        var newPassword = "newPassword";
        var credential = (Encoding.UTF8.GetBytes("salt"), Encoding.UTF8.GetBytes("hashedPassword"));
        var brandId = Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af");
        var eventId = Guid.NewGuid();

        _ = fakePasswordHandler
            .GenerateCredential(newPassword)
            .Returns(credential);

        _ = fakeSerialNumberGenerator.GenerateAsync()
            .Returns(eventId);

        _ = fakeAgentRepository.GetAgentByIdAsync(agentId)
            .Returns(new AgentInfo(
                Id: agentId,
                Username: "agentUsername",
                null,
                null,
                null,
                null,
                null,
                "inviteCode",
                null,
                DateTimeOffset.UtcNow.AddMonths(-1),
                AgentStatus.Enabled,
                AgentType.Cash,
                AgentLevelType.Chief,
                0m,
                0,
                0m,
                brandId,
                Guid.NewGuid(),
                null,
                false));


        var sut = new SetAgentPayPasswordCommandService(fakeAgentRepository, fakePasswordHandler, fakeSerialNumberGenerator);

        await sut.ExecuteAsync(new SetAgentPayPasswordCommand(agentId, newPassword, modifyUserId));

        _ = fakeAgentRepository
            .Received(1)
            .SetPayPasswordAsync(Arg.Is(agentId), Arg.Is(credential.Item2), Arg.Is(credential.Item1));
    }
}