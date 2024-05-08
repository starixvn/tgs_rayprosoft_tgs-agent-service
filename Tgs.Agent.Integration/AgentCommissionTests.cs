using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;
using Tgs.Agent.Client;
using Tgs.Agent.Core;
using Tgs.Agent.Core.ApplicationServices;
using Tgs.Agent.Repositories.Entities;
using Tgs.ServiceUtility;

namespace Tgs.Agent.IntegrationTest;

public class AgentCommission
{
    [Test]
    public async Task Clear_Agent_Negative_Commission()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();
        var fakeCommand = Substitute.For<ICommandService<ClearNegativeCommissionCommand>>();
        await using var web = new AgentTestApplication(builder
            => builder.ConfigureServices(services => services.AddSingleton(fakeAgentRepository)
                .AddSingleton(fakeCommand)));

        var httpClient = web.CreateClient();

        var sut = new AgentClient(httpClient);

        var agentId = Guid.Parse("8480c811-879b-41e0-8b8b-bd0c9cdbdf31");
        var clearByUserId = Guid.Parse("8480c811-879b-41e0-8b8b-bd0c9cdbdf33");

        await sut.ClearAgentNegativeCommissionAsync(new(agentId, clearByUserId));

        await fakeCommand.Received(1)
            .ExecuteAsync(Arg.Any<ClearNegativeCommissionCommand>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Clear_Agent_Negative_Commission_And_Set_Send_On()
    {
        var fakeTgsContext = Substitute.For<TgsContext>();
        var fakeDbSet = Substitute.For<DbSet<AgentDividend>, IQueryable<AgentDividend>>();

        var agentId = Guid.Parse("8480c811-879b-41e0-8b8b-bd0c9cdbdf31");
        var clearByUserId = Guid.Parse("8480c811-879b-41e0-8b8b-bd0c9cdbdf33");

        var agentDividends = new AgentDividend[]
        {
            new()
            {
                Id = 1,
                AgentId = agentId,
                BrandId = Guid.NewGuid(),
                AgentDividendAmount = 1,
                SentOn = null
            },
            new()
            {
                Id = 2,
                AgentId = agentId,
                BrandId = Guid.NewGuid(),
                AgentDividendAmount = -10,
                BeforeDividendAmount = null,
                ReferencedAgentDividendId = null,
                SentOn = null
            }
        };

        var fakeAgentDividends = fakeDbSet.Initialize(agentDividends.AsQueryable());

        _ = fakeTgsContext.AgentDividends.Returns(fakeAgentDividends);

        await using var web = new AgentTestApplication(builder
            => builder.ConfigureServices(services => services.AddSingleton(fakeTgsContext)));

        var httpClient = web.CreateClient();

        var sut = new AgentClient(httpClient);

        await sut.ClearAgentNegativeCommissionAsync(new(agentId, clearByUserId));

        await fakeTgsContext.Received(1)
            .AddRangeAsync(Arg.Any<IEnumerable<AgentClearCommissionRecord>>(), Arg.Any<CancellationToken>());

        fakeTgsContext.AgentDividends.Received(1).UpdateRange(Arg.Any<IEnumerable<AgentDividend>>());
    }
}

public static class ExtensionMethods
{
    public static DbSet<T> Initialize<T>(this DbSet<T> dbSet, IQueryable<T> data) where T : class
    {
        _ = ((IQueryable<T>)dbSet).Provider.Returns(data.Provider);
        _ = ((IQueryable<T>)dbSet).Expression.Returns(data.Expression);
        _ = ((IQueryable<T>)dbSet).ElementType.Returns(data.ElementType);
        _ = ((IQueryable<T>)dbSet).GetEnumerator().Returns(data.GetEnumerator());

        return dbSet;
    }
}