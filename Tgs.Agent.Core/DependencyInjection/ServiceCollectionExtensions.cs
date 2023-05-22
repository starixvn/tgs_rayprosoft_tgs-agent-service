// ReSharper disable CheckNamespace

using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Tgs.Agent.Core.RepositoryModels;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepository(this IServiceCollection services,
        Action<IServiceProvider, DbContextOptionsBuilder> dbContextConfigure)
        => services
            .AddDbContext<tgsContext>(dbContextConfigure)
            .RegisterAllServiceUtilityTypes(Assembly.GetExecutingAssembly(), ServiceLifetime.Transient);
}