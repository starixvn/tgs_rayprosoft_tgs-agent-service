using Tgs.Agent.Client;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IHttpClientBuilder AddAgentClient(this IServiceCollection services) 
        => services.AddHttpClient<IAgentClient, AgentClient>();
}