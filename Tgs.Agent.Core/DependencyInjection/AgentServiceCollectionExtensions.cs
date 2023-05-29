namespace Microsoft.Extensions.DependencyInjection;

public static class AgentServiceCollectionExtensions
{
    public static IServiceCollection AddAgentCore(this IServiceCollection services)
    {
        _ = services.RegisterAllServiceUtilityTypes(
            typeof(AgentServiceCollectionExtensions).Assembly,
            ServiceLifetime.Transient);

        return services;
    }
}