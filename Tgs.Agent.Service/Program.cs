using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddHttpClient()
    .AddCore()
    .AddRepository((sp, optionsBuilder) =>
    {
        var connectionString = sp.GetRequiredService<IConfiguration>().GetConnectionString("Tgs");
        _ = optionsBuilder.UseNpgsql(connectionString);
    });

builder.Logging
    .AddOpenTelemetry(options =>
    {
        options.IncludeFormattedMessage = true;
        options.ParseStateValues = true;
        _ = options.AddFluentdForwardExporter(fluentOptions =>
        {
            var nodeIp = builder.Configuration.GetValue<string>("NODE_IP");
            fluentOptions.Host = nodeIp!;
            fluentOptions.Tag = builder.Configuration.GetValue<string>("OTEL_SERVICE_NAME")!;
            fluentOptions.UseMessagePack();
        });
    });

builder.Services
    .AddOptions<OtlpExporterOptions>()
    .Configure<IConfiguration>((options, conifg) =>
    {
        var nodeIp = conifg.GetValue<string>("NODE_IP");

        if (!string.IsNullOrWhiteSpace(nodeIp))
        {
            options.Endpoint = new($"http://{nodeIp}:4317");
        }

        conifg.GetSection("Opentelemetry:Tracing").Bind(options);
    });

builder.Services
    .AddOpenTelemetryTracing(builder => builder
        .SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddEnvironmentVariableDetector())
        .AddSource("Tgs.*")
        .AddSource("TGS.*")
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddEntityFrameworkCoreInstrumentation(options =>
        {
            options.SetDbStatementForText = true;
        })
        .AddProfileViewExporter()
        .AddOtlpExporter())
    .Configure<AspNetCoreInstrumentationOptions>(options =>
    {
        options.Filter = httpContext =>
        {
            return !httpContext.Request.Path.HasValue
                || (!httpContext.Request.Path.StartsWithSegments("/swagger")
                    && !httpContext.Request.Path.StartsWithSegments("/healthz")
                    && !httpContext.Request.Path.StartsWithSegments("/profiler/")
                    && !new[]
                    {
                        "html",
                        "ico",
                        "png",
                        "jpg"
                    }.Any(
                        ext => httpContext.Request.Path.Value.EndsWith($".{ext}")));
        };
    });
builder.Services
    .AddProfileViewer()
    .AddInterceptionService()
    .AddServiceUtiltiyInterceptors();

builder.Services
    .AddSerialNumberGenerator()
    .AddCore()
    .AddPlayerServiceClient((options, sp) =>
    {
        var config = sp.GetRequiredService<IConfiguration>();
        options.BaseUrl = config.GetValue<Uri>("AgentService:PlayerServiceBaseUrl")!;
    })
    .AddSecurity();

builder.Services.AddHealthChecks();

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwaggerUI();

app.MapControllers();
app.MapSwagger();
app.MapHealthChecks("/healthz");
app.MapProfileViewer();

app.Run();