// ReSharper disable CheckNamespace

using Microsoft.EntityFrameworkCore;
using Tgs.Agent.Core;
using Tgs.Agent.Repositories.RepositoryModels;
using TGS.Agent.Repositories;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepository(this IServiceCollection services,
        Action<IServiceProvider, DbContextOptionsBuilder> dbContextConfigure)
        => services
            .AddDbContext<tgsContext>(dbContextConfigure)
            .AddTransient<IAgentRepository, AgentRepository>();
}