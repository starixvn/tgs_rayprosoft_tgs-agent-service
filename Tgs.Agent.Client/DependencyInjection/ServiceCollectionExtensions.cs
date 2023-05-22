using Microsoft.Extensions.Options;
using Tgs.Agent.Client;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAgentClient(this IServiceCollection services,
        Action<AgentOptions, IServiceProvider> configure)
    {
        services.AddOptions<AgentOptions>().Configure(configure);

        services.AddHttpClient<IAgentClient, AgentClient>()
            .ConfigureHttpClient((sp, httpClient) =>
            {
                var options = sp.GetRequiredService<IOptions<AgentOptions>>().Value;
                httpClient.BaseAddress = options.BaseUrl;
            });

        return services;
    }
}