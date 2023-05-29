using Microsoft.EntityFrameworkCore;
using Tgs.Agent;
using Tgs.Agent.Repositories;
using Tgs.Agent.Repositories.Entities;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepository(this IServiceCollection services,
        Action<IServiceProvider, DbContextOptionsBuilder> dbContextConfigure)
        => services
            .AddDbContext<TgsContext>(dbContextConfigure)
            .AddTransient<IAgentRepository, AgentRepository>();
}