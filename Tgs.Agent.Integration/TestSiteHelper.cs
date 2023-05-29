using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using OpenTelemetry.Exporter;
using Tgs.Agent.Repositories.Entities;

namespace Tgs.Agent.IntegrationTest;

internal static class TestSiteHelper
{
    public static TestAgentApplication GetAgentServiceTestHost(Action<IServiceCollection> configureServices)
        => new(webBuilder => _ = webBuilder
            .ConfigureAppConfiguration((ctx, builder) => builder.Sources.Clear())
            .ConfigureServices(services =>
            {
                _ = services.AddSingleton<IConfigureOptions<OtlpExporterOptions>>(
                    sp => new ConfigureNamedOptions<OtlpExporterOptions>(
                        Options.DefaultName,
                        exporterOptions =>
                        {
                            exporterOptions.Endpoint = null;
                            exporterOptions.TimeoutMilliseconds = 0;
                        }))
                    .Replace(ServiceDescriptor.Singleton(sp =>
                    {
                        var dbContextOptionsBuilder = new DbContextOptionsBuilder<TgsContext>();
                        _ = dbContextOptionsBuilder
                            .UseInMemoryDatabase(Guid.NewGuid().ToString())
                            .ConfigureWarnings(x => x.Ignore(
                                InMemoryEventId.TransactionIgnoredWarning));

                        return new TgsContext(dbContextOptionsBuilder.Options);
                    }));

                configureServices(services);
            }));
}