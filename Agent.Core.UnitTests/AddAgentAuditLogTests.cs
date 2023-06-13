using NSubstitute;
using Tgs.Agent.Core;
using Tgs.Agent.Core.ApplicationServices;
using Tgs.Agent.Core.Models.Enums;

namespace Agent.Core.UnitTests;

public class AddAgentAuditLogTests
{
    [Test]
    public async Task Add_Agent_Audit_Log()
    {
        var fakeAgentRepository = Substitute.For<IAgentAuditLogRepository>();

        var sut = new AddAgentAuditLogCommandService(fakeAgentRepository);

        var command = new AddAgentAuditLogCommand(Guid.NewGuid(), "editUsername", AgentOperation.Info);

        await sut.ExecuteAsync(command);

        _ = fakeAgentRepository
            .Received(1)
            .AddAgentAuditLogAsync(Arg.Is(command.AgentId), Arg.Is(command.Operation), Arg.Is(command.EditUsername));
    }
}