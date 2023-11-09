using System.Collections.Immutable;
using System.Text.Json;
using NSubstitute;
using Tgs.Agent.Core;
using Tgs.Agent.Core.ApplicationServices;
using Tgs.Agent.Core.Models;
using Tgs.SerialNumberGenerator;

namespace Agent.Core.UnitTests;

public class SetAgentPropertiesTests
{
    [Test]
    public async Task 更新代理真實姓名()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();
        var fakeDataCommandBuilder = Substitute.For<IDataCommandBuilder>();
        var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();

        var brandId = Guid.NewGuid();
        var agentId = Guid.NewGuid();
        var realName = "agentRealName";
        var modifyUserId = "50ce61eb-8c9d-4f84-3cf6-08d9262ce1a5";

        var agentInfo = new AgentInfo(
            agentId,
            "agentUsername",
            RealName: null,
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

        var agentProperties = new Dictionary<string, string?>
        {
            ["RealName"] = realName
        };

        var sut = new SetAgentPropertiesCommandService(fakeAgentRepository, fakeDataCommandBuilder, fakeSerialNumberGenerator);

        var command = new SetAgentPropertiesCommand(agentId, agentProperties.ToImmutableDictionary(), modifyUserId);

        _ = fakeAgentRepository
            .GetAgentByIdAsync(agentId)
            .Returns(agentInfo);

        var eventId = Guid.NewGuid();
        _ = fakeSerialNumberGenerator
            .GenerateAsync(Arg.Any<CancellationToken>())
            .Returns(eventId);

        await sut.ExecuteAsync(command).ConfigureAwait(false);

        _ = fakeDataCommandBuilder
            .Received(1)
            .CreatePutCommand(Arg.Is(nameof(AgentPropertyName.RealName)), Arg.Is(realName));

        _ = fakeAgentRepository
            .Received(1)
            .SetAgentPropertiesAsync(
                agentInfo.BrandId,
                agentInfo.Id,
                Arg.Is<IEnumerable<IDataCommand>>(commands => commands.Count() == 1),
                Arg.Any<CancellationToken>());

        _ = fakeAgentRepository
            .Received(1)
            .CreateAgentEventAsync(
                Arg.Is(eventId),
                Arg.Is(brandId),
                Arg.Is(agentId),
                Arg.Is(EventAction.Update),
                Arg.Any<JsonDocument>(),
                Arg.Is(modifyUserId),
                Arg.Any<CancellationToken>());

        _ = fakeAgentRepository
            .Received(1)
            .UpdateAgentSnapshotAsync(
                Arg.Is(agentId),
                Arg.Is(eventId),
                Arg.Any<DateTime>(),
                Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task 更新代理備註()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();
        var fakeDataCommandBuilder = Substitute.For<IDataCommandBuilder>();
        var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();

        var brandId = Guid.NewGuid();
        var agentId = Guid.NewGuid();
        var remark = "test remark";
        var modifyUserId = "50ce61eb-8c9d-4f84-3cf6-08d9262ce1a5";

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

        var agentProperties = new Dictionary<string, string?>
        {
            ["Remarks"] = remark
        };

        var sut = new SetAgentPropertiesCommandService(fakeAgentRepository, fakeDataCommandBuilder, fakeSerialNumberGenerator);

        var command = new SetAgentPropertiesCommand(agentId, agentProperties.ToImmutableDictionary(), modifyUserId);

        _ = fakeAgentRepository
            .GetAgentByIdAsync(agentId, Arg.Any<CancellationToken>())
            .Returns(agentInfo);

        var eventId = Guid.NewGuid();
        _ = fakeSerialNumberGenerator
            .GenerateAsync(Arg.Any<CancellationToken>())
            .Returns(eventId);

        await sut.ExecuteAsync(command).ConfigureAwait(false);

        _ = fakeDataCommandBuilder
            .Received(1)
            .CreatePutCommand(Arg.Is(nameof(AgentPropertyName.Remarks)), Arg.Is(remark));

        _ = fakeAgentRepository
            .Received(1)
            .SetAgentPropertiesAsync(
                agentInfo.BrandId,
                agentInfo.Id,
                Arg.Is<IEnumerable<IDataCommand>>(commands => commands.Count() == 1),
                Arg.Any<CancellationToken>());

        _ = fakeAgentRepository
            .Received(1)
            .CreateAgentEventAsync(
                Arg.Is(eventId),
                Arg.Is(brandId),
                Arg.Is(agentId),
                Arg.Is(EventAction.Update),
                Arg.Any<JsonDocument>(),
                Arg.Is(modifyUserId),
                Arg.Any<CancellationToken>());

        _ = fakeAgentRepository
            .Received(1)
            .UpdateAgentSnapshotAsync(
                Arg.Is(agentId),
                Arg.Is(eventId),
                Arg.Any<DateTime>(),
                Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task 更新代理手機號碼()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();
        var fakeDataCommandBuilder = Substitute.For<IDataCommandBuilder>();
        var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();

        var brandId = Guid.NewGuid();
        var agentId = Guid.NewGuid();
        var phone = "testPhone";
        var modifyUserId = "50ce61eb-8c9d-4f84-3cf6-08d9262ce1a5";

        var agentInfo = new AgentInfo(
            agentId,
            "agentUsername",
            null,
            Phone: null,
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

        var agentProperties = new Dictionary<string, string?>
        {
            ["Phone"] = phone
        };

        var sut = new SetAgentPropertiesCommandService(fakeAgentRepository, fakeDataCommandBuilder, fakeSerialNumberGenerator);

        var command = new SetAgentPropertiesCommand(agentId, agentProperties.ToImmutableDictionary(), modifyUserId);

        _ = fakeAgentRepository
            .GetAgentByIdAsync(agentId, Arg.Any<CancellationToken>())
            .Returns(agentInfo);

        var eventId = Guid.NewGuid();
        _ = fakeSerialNumberGenerator
            .GenerateAsync(Arg.Any<CancellationToken>())
            .Returns(eventId);

        await sut.ExecuteAsync(command).ConfigureAwait(false);

        _ = fakeDataCommandBuilder
            .Received(1)
            .CreatePutCommand(Arg.Is(nameof(AgentPropertyName.Phone)), Arg.Is(phone));

        _ = fakeAgentRepository
            .Received(1)
            .SetAgentPropertiesAsync(
                agentInfo.BrandId,
                agentInfo.Id,
                Arg.Is<IEnumerable<IDataCommand>>(commands => commands.Count() == 1),
                Arg.Any<CancellationToken>());

        _ = fakeAgentRepository
            .Received(1)
            .CreateAgentEventAsync(
                Arg.Is(eventId),
                Arg.Is(brandId),
                Arg.Is(agentId),
                Arg.Is(EventAction.Update),
                Arg.Any<JsonDocument>(),
                Arg.Is(modifyUserId),
                Arg.Any<CancellationToken>());

        _ = fakeAgentRepository
            .Received(1)
            .UpdateAgentSnapshotAsync(
                Arg.Is(agentId),
                Arg.Is(eventId),
                Arg.Any<DateTime>(),
                Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task 更新代理郵箱()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();
        var fakeDataCommandBuilder = Substitute.For<IDataCommandBuilder>();
        var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();

        var brandId = Guid.NewGuid();
        var agentId = Guid.NewGuid();
        var email = "testMail@test.com";
        var modifyUserId = "50ce61eb-8c9d-4f84-3cf6-08d9262ce1a5";

        var agentInfo = new AgentInfo(
            agentId,
            "agentUsername",
            null,
            null,
            Email: null,
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

        var agentProperties = new Dictionary<string, string?>
        {
            ["Email"] = email
        };

        var sut = new SetAgentPropertiesCommandService(fakeAgentRepository, fakeDataCommandBuilder, fakeSerialNumberGenerator);

        var command = new SetAgentPropertiesCommand(agentId, agentProperties.ToImmutableDictionary(), modifyUserId);

        _ = fakeAgentRepository
            .GetAgentByIdAsync(agentId, Arg.Any<CancellationToken>())
            .Returns(agentInfo);

        var eventId = Guid.NewGuid();
        _ = fakeSerialNumberGenerator
            .GenerateAsync(Arg.Any<CancellationToken>())
            .Returns(eventId);

        await sut.ExecuteAsync(command).ConfigureAwait(false);

        _ = fakeDataCommandBuilder
            .Received(1)
            .CreatePutCommand(Arg.Is(nameof(AgentPropertyName.Email)), Arg.Is(email));

        _ = fakeAgentRepository
            .Received(1)
            .SetAgentPropertiesAsync(
                agentInfo.BrandId,
                agentInfo.Id,
                Arg.Is<IEnumerable<IDataCommand>>(commands => commands.Count() == 1),
                Arg.Any<CancellationToken>());

        _ = fakeAgentRepository
            .Received(1)
            .CreateAgentEventAsync(
                Arg.Is(eventId),
                Arg.Is(brandId),
                Arg.Is(agentId),
                Arg.Is(EventAction.Update),
                Arg.Any<JsonDocument>(),
                Arg.Is(modifyUserId),
                Arg.Any<CancellationToken>());

        _ = fakeAgentRepository
            .Received(1)
            .UpdateAgentSnapshotAsync(
                Arg.Is(agentId),
                Arg.Is(eventId),
                Arg.Any<DateTime>(),
                Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task 更新代理Qq()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();
        var fakeDataCommandBuilder = Substitute.For<IDataCommandBuilder>();
        var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();

        var brandId = Guid.NewGuid();
        var agentId = Guid.NewGuid();
        var qq = "testQq";
        var modifyUserId = "50ce61eb-8c9d-4f84-3cf6-08d9262ce1a5";

        var agentInfo = new AgentInfo(
            agentId,
            "agentUsername",
            null,
            null,
            null,
            QQ: null,
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

        var agentProperties = new Dictionary<string, string?>
        {
            ["Qq"] = qq
        };

        var sut = new SetAgentPropertiesCommandService(fakeAgentRepository, fakeDataCommandBuilder, fakeSerialNumberGenerator);

        var command = new SetAgentPropertiesCommand(agentId, agentProperties.ToImmutableDictionary(), modifyUserId);

        _ = fakeAgentRepository
            .GetAgentByIdAsync(agentId, Arg.Any<CancellationToken>())
            .Returns(agentInfo);

        var eventId = Guid.NewGuid();
        _ = fakeSerialNumberGenerator
            .GenerateAsync(Arg.Any<CancellationToken>())
            .Returns(eventId);

        await sut.ExecuteAsync(command).ConfigureAwait(false);

        _ = fakeDataCommandBuilder
            .Received(1)
            .CreatePutCommand(Arg.Is(nameof(AgentPropertyName.Qq)), Arg.Is(qq));

        _ = fakeAgentRepository
            .Received(1)
            .SetAgentPropertiesAsync(
                agentInfo.BrandId,
                agentInfo.Id,
                Arg.Is<IEnumerable<IDataCommand>>(commands => commands.Count() == 1),
                Arg.Any<CancellationToken>());

        _ = fakeAgentRepository
            .Received(1)
            .CreateAgentEventAsync(
                Arg.Is(eventId),
                Arg.Is(brandId),
                Arg.Is(agentId),
                Arg.Is(EventAction.Update),
                Arg.Any<JsonDocument>(),
                Arg.Is(modifyUserId),
                Arg.Any<CancellationToken>());

        _ = fakeAgentRepository
            .Received(1)
            .UpdateAgentSnapshotAsync(
                Arg.Is(agentId),
                Arg.Is(eventId),
                Arg.Any<DateTime>(),
                Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task 更新代理Wechat()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();
        var fakeDataCommandBuilder = Substitute.For<IDataCommandBuilder>();
        var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();

        var brandId = Guid.NewGuid();
        var agentId = Guid.NewGuid();
        var wechat = "testWechat";
        var modifyUserId = "50ce61eb-8c9d-4f84-3cf6-08d9262ce1a5";

        var agentInfo = new AgentInfo(
            agentId,
            "agentUsername",
            null,
            null,
            null,
            null,
            Wechat: null,
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

        var agentProperties = new Dictionary<string, string?>
        {
            ["Wechat"] = wechat
        };

        var sut = new SetAgentPropertiesCommandService(fakeAgentRepository, fakeDataCommandBuilder, fakeSerialNumberGenerator);

        var command = new SetAgentPropertiesCommand(agentId, agentProperties.ToImmutableDictionary(), modifyUserId);

        _ = fakeAgentRepository
            .GetAgentByIdAsync(agentId, Arg.Any<CancellationToken>())
            .Returns(agentInfo);

        var eventId = Guid.NewGuid();
        _ = fakeSerialNumberGenerator
            .GenerateAsync(Arg.Any<CancellationToken>())
            .Returns(eventId);

        await sut.ExecuteAsync(command).ConfigureAwait(false);

        _ = fakeDataCommandBuilder
            .Received(1)
            .CreatePutCommand(Arg.Is(nameof(AgentPropertyName.Wechat)), Arg.Is(wechat));

        _ = fakeAgentRepository
            .Received(1)
            .SetAgentPropertiesAsync(
                agentInfo.BrandId,
                agentInfo.Id,
                Arg.Is<IEnumerable<IDataCommand>>(commands => commands.Count() == 1),
                Arg.Any<CancellationToken>());

        _ = fakeAgentRepository
            .Received(1)
            .CreateAgentEventAsync(
                Arg.Is(eventId),
                Arg.Is(brandId),
                Arg.Is(agentId),
                Arg.Is(EventAction.Update),
                Arg.Any<JsonDocument>(),
                Arg.Is(modifyUserId),
                Arg.Any<CancellationToken>());

        _ = fakeAgentRepository
            .Received(1)
            .UpdateAgentSnapshotAsync(
                Arg.Is(agentId),
                Arg.Is(eventId),
                Arg.Any<DateTime>(),
                Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task 刪除代理真實姓名()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();
        var fakeDataCommandBuilder = Substitute.For<IDataCommandBuilder>();
        var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();

        var brandId = Guid.NewGuid();
        var agentId = Guid.NewGuid();
        var modifyUserId = "50ce61eb-8c9d-4f84-3cf6-08d9262ce1a5";

        var agentInfo = new AgentInfo(
            agentId,
            "agentUsername",
            RealName: null,
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

        var agentProperties = new Dictionary<string, string?>
        {
            ["RealName"] = null
        };

        var sut = new SetAgentPropertiesCommandService(fakeAgentRepository, fakeDataCommandBuilder, fakeSerialNumberGenerator);

        var command = new SetAgentPropertiesCommand(agentId, agentProperties.ToImmutableDictionary(), modifyUserId);

        _ = fakeAgentRepository
            .GetAgentByIdAsync(agentId, Arg.Any<CancellationToken>())
            .Returns(agentInfo);

        var eventId = Guid.NewGuid();
        _ = fakeSerialNumberGenerator
            .GenerateAsync(Arg.Any<CancellationToken>())
            .Returns(eventId);

        await sut.ExecuteAsync(command).ConfigureAwait(false);

        _ = fakeDataCommandBuilder
            .Received(1)
            .CreateDeleteCommand(Arg.Is(nameof(AgentPropertyName.RealName)));

        _ = fakeAgentRepository
            .Received(1)
            .SetAgentPropertiesAsync(
                agentInfo.BrandId,
                agentInfo.Id,
                Arg.Is<IEnumerable<IDataCommand>>(commands => commands.Count() == 1),
                Arg.Any<CancellationToken>());

        _ = fakeAgentRepository
            .Received(1)
            .CreateAgentEventAsync(
                Arg.Is(eventId),
                Arg.Is(brandId),
                Arg.Is(agentId),
                Arg.Is(EventAction.Update),
                Arg.Any<JsonDocument>(),
                Arg.Is(modifyUserId),
                Arg.Any<CancellationToken>());

        _ = fakeAgentRepository
            .Received(1)
            .UpdateAgentSnapshotAsync(
                Arg.Is(agentId),
                Arg.Is(eventId),
                Arg.Any<DateTime>(),
                Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task 刪除代理備註()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();
        var fakeDataCommandBuilder = Substitute.For<IDataCommandBuilder>();
        var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();

        var brandId = Guid.NewGuid();
        var agentId = Guid.NewGuid();
        var modifyUserId = "50ce61eb-8c9d-4f84-3cf6-08d9262ce1a5";

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

        var agentProperties = new Dictionary<string, string?>
        {
            ["Remarks"] = null
        };

        var sut = new SetAgentPropertiesCommandService(fakeAgentRepository, fakeDataCommandBuilder, fakeSerialNumberGenerator);

        var command = new SetAgentPropertiesCommand(agentId, agentProperties.ToImmutableDictionary(), modifyUserId);

        _ = fakeAgentRepository
            .GetAgentByIdAsync(agentId, Arg.Any<CancellationToken>())
            .Returns(agentInfo);

        var eventId = Guid.NewGuid();
        _ = fakeSerialNumberGenerator
            .GenerateAsync(Arg.Any<CancellationToken>())
            .Returns(eventId);

        await sut.ExecuteAsync(command).ConfigureAwait(false);

        _ = fakeDataCommandBuilder
            .Received(1)
            .CreateDeleteCommand(Arg.Is(nameof(AgentPropertyName.Remarks)));

        _ = fakeAgentRepository
            .Received(1)
            .SetAgentPropertiesAsync(
                agentInfo.BrandId,
                agentInfo.Id,
                Arg.Is<IEnumerable<IDataCommand>>(commands => commands.Count() == 1),
                Arg.Any<CancellationToken>());

        _ = fakeAgentRepository
            .Received(1)
            .CreateAgentEventAsync(
                Arg.Is(eventId),
                Arg.Is(brandId),
                Arg.Is(agentId),
                Arg.Is(EventAction.Update),
                Arg.Any<JsonDocument>(),
                Arg.Is(modifyUserId),
                Arg.Any<CancellationToken>());

        _ = fakeAgentRepository
            .Received(1)
            .UpdateAgentSnapshotAsync(
                Arg.Is(agentId),
                Arg.Is(eventId),
                Arg.Any<DateTime>(),
                Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task 刪除代理手機號碼()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();
        var fakeDataCommandBuilder = Substitute.For<IDataCommandBuilder>();
        var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();

        var brandId = Guid.NewGuid();
        var agentId = Guid.NewGuid();
        var modifyUserId = "50ce61eb-8c9d-4f84-3cf6-08d9262ce1a5";

        var agentInfo = new AgentInfo(
            agentId,
            "agentUsername",
            null,
            Phone: null,
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

        var agentProperties = new Dictionary<string, string?>
        {
            ["Phone"] = null
        };

        var sut = new SetAgentPropertiesCommandService(fakeAgentRepository, fakeDataCommandBuilder, fakeSerialNumberGenerator);

        var command = new SetAgentPropertiesCommand(agentId, agentProperties.ToImmutableDictionary(), modifyUserId);

        _ = fakeAgentRepository
            .GetAgentByIdAsync(agentId, Arg.Any<CancellationToken>())
            .Returns(agentInfo);

        var eventId = Guid.NewGuid();
        _ = fakeSerialNumberGenerator
            .GenerateAsync(Arg.Any<CancellationToken>())
            .Returns(eventId);

        await sut.ExecuteAsync(command).ConfigureAwait(false);

        _ = fakeDataCommandBuilder
            .Received(1)
            .CreateDeleteCommand(Arg.Is(nameof(AgentPropertyName.Phone)));

        _ = fakeAgentRepository
            .Received(1)
            .SetAgentPropertiesAsync(
                agentInfo.BrandId,
                agentInfo.Id,
                Arg.Is<IEnumerable<IDataCommand>>(commands => commands.Count() == 1),
                Arg.Any<CancellationToken>());

        _ = fakeAgentRepository
            .Received(1)
            .CreateAgentEventAsync(
                Arg.Is(eventId),
                Arg.Is(brandId),
                Arg.Is(agentId),
                Arg.Is(EventAction.Update),
                Arg.Any<JsonDocument>(),
                Arg.Is(modifyUserId),
                Arg.Any<CancellationToken>());

        _ = fakeAgentRepository
            .Received(1)
            .UpdateAgentSnapshotAsync(
                Arg.Is(agentId),
                Arg.Is(eventId),
                Arg.Any<DateTime>(),
                Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task 刪除代理郵箱()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();
        var fakeDataCommandBuilder = Substitute.For<IDataCommandBuilder>();
        var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();

        var brandId = Guid.NewGuid();
        var agentId = Guid.NewGuid();
        var modifyUserId = "50ce61eb-8c9d-4f84-3cf6-08d9262ce1a5";

        var agentInfo = new AgentInfo(
            agentId,
            "agentUsername",
            null,
            null,
            Email: null,
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

        var agentProperties = new Dictionary<string, string?>
        {
            ["Email"] = null
        };

        var sut = new SetAgentPropertiesCommandService(fakeAgentRepository, fakeDataCommandBuilder, fakeSerialNumberGenerator);

        var command = new SetAgentPropertiesCommand(agentId, agentProperties.ToImmutableDictionary(), modifyUserId);

        _ = fakeAgentRepository
            .GetAgentByIdAsync(agentId, Arg.Any<CancellationToken>())
            .Returns(agentInfo);

        var eventId = Guid.NewGuid();
        _ = fakeSerialNumberGenerator
            .GenerateAsync(Arg.Any<CancellationToken>())
            .Returns(eventId);

        await sut.ExecuteAsync(command).ConfigureAwait(false);

        _ = fakeDataCommandBuilder
            .Received(1)
            .CreateDeleteCommand(Arg.Is(nameof(AgentPropertyName.Email)));

        _ = fakeAgentRepository
            .Received(1)
            .SetAgentPropertiesAsync(
                agentInfo.BrandId,
                agentInfo.Id,
                Arg.Is<IEnumerable<IDataCommand>>(commands => commands.Count() == 1),
                Arg.Any<CancellationToken>());

        _ = fakeAgentRepository
            .Received(1)
            .CreateAgentEventAsync(
                Arg.Is(eventId),
                Arg.Is(brandId),
                Arg.Is(agentId),
                Arg.Is(EventAction.Update),
                Arg.Any<JsonDocument>(),
                Arg.Is(modifyUserId),
                Arg.Any<CancellationToken>());

        _ = fakeAgentRepository
            .Received(1)
            .UpdateAgentSnapshotAsync(
                Arg.Is(agentId),
                Arg.Is(eventId),
                Arg.Any<DateTime>(),
                Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task 刪除代理Qq()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();
        var fakeDataCommandBuilder = Substitute.For<IDataCommandBuilder>();
        var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();

        var brandId = Guid.NewGuid();
        var agentId = Guid.NewGuid();
        var modifyUserId = "50ce61eb-8c9d-4f84-3cf6-08d9262ce1a5";

        var agentInfo = new AgentInfo(
            agentId,
            "agentUsername",
            null,
            null,
            null,
            QQ: null,
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

        var agentProperties = new Dictionary<string, string?>
        {
            ["Qq"] = null
        };

        var sut = new SetAgentPropertiesCommandService(fakeAgentRepository, fakeDataCommandBuilder, fakeSerialNumberGenerator);

        var command = new SetAgentPropertiesCommand(agentId, agentProperties.ToImmutableDictionary(), modifyUserId);

        _ = fakeAgentRepository
            .GetAgentByIdAsync(agentId, Arg.Any<CancellationToken>())
            .Returns(agentInfo);

        var eventId = Guid.NewGuid();
        _ = fakeSerialNumberGenerator
            .GenerateAsync(Arg.Any<CancellationToken>())
            .Returns(eventId);

        await sut.ExecuteAsync(command).ConfigureAwait(false);

        _ = fakeDataCommandBuilder
            .Received(1)
            .CreateDeleteCommand(Arg.Is(nameof(AgentPropertyName.Qq)));

        _ = fakeAgentRepository
            .Received(1)
            .SetAgentPropertiesAsync(
                agentInfo.BrandId,
                agentInfo.Id,
                Arg.Is<IEnumerable<IDataCommand>>(commands => commands.Count() == 1),
                Arg.Any<CancellationToken>());

        _ = fakeAgentRepository
            .Received(1)
            .CreateAgentEventAsync(
                Arg.Is(eventId),
                Arg.Is(brandId),
                Arg.Is(agentId),
                Arg.Is(EventAction.Update),
                Arg.Any<JsonDocument>(),
                Arg.Is(modifyUserId),
                Arg.Any<CancellationToken>());

        _ = fakeAgentRepository
            .Received(1)
            .UpdateAgentSnapshotAsync(
                Arg.Is(agentId),
                Arg.Is(eventId),
                Arg.Any<DateTime>(),
                Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task 刪除代理Wechat()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();
        var fakeDataCommandBuilder = Substitute.For<IDataCommandBuilder>();
        var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();

        var brandId = Guid.NewGuid();
        var agentId = Guid.NewGuid();
        var modifyUserId = "50ce61eb-8c9d-4f84-3cf6-08d9262ce1a5";

        var agentInfo = new AgentInfo(
            agentId,
            "agentUsername",
            null,
            null,
            null,
            null,
            Wechat: null,
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

        var agentProperties = new Dictionary<string, string?>
        {
            ["Wechat"] = null
        };

        var sut = new SetAgentPropertiesCommandService(fakeAgentRepository, fakeDataCommandBuilder, fakeSerialNumberGenerator);

        var command = new SetAgentPropertiesCommand(agentId, agentProperties.ToImmutableDictionary(), modifyUserId);

        _ = fakeAgentRepository
            .GetAgentByIdAsync(agentId, Arg.Any<CancellationToken>())
            .Returns(agentInfo);

        var eventId = Guid.NewGuid();
        _ = fakeSerialNumberGenerator
            .GenerateAsync(Arg.Any<CancellationToken>())
            .Returns(eventId);

        await sut.ExecuteAsync(command).ConfigureAwait(false);

        _ = fakeDataCommandBuilder
            .Received(1)
            .CreateDeleteCommand(Arg.Is(nameof(AgentPropertyName.Wechat)));

        _ = fakeAgentRepository
            .Received(1)
            .SetAgentPropertiesAsync(
                agentInfo.BrandId,
                agentInfo.Id,
                Arg.Is<IEnumerable<IDataCommand>>(commands => commands.Count() == 1),
                Arg.Any<CancellationToken>());

        _ = fakeAgentRepository
            .Received(1)
            .CreateAgentEventAsync(
                Arg.Is(eventId),
                Arg.Is(brandId),
                Arg.Is(agentId),
                Arg.Is(EventAction.Update),
                Arg.Any<JsonDocument>(),
                Arg.Is(modifyUserId),
                Arg.Any<CancellationToken>());

        _ = fakeAgentRepository
            .Received(1)
            .UpdateAgentSnapshotAsync(
                Arg.Is(agentId),
                Arg.Is(eventId),
                Arg.Any<DateTime>(),
                Arg.Any<CancellationToken>());
    }
}