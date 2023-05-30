using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Tgs.Agent.Client.Models;

namespace Tgs.Agent.Client;

public class AgentClient : IAgentClient
{
    private readonly HttpClient _client;

    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public AgentClient(HttpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<AgentInfoModel?> GetAgentInfoAsync(GetAgentInfoRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _client
            .GetAsync($"/api/agent/{request.AgentId}", cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        if (string.IsNullOrEmpty(contentJson))
            return null;

        return JsonSerializer.Deserialize<AgentInfoModel>(contentJson, _jsonSerializerOptions);
    }

    public async IAsyncEnumerable<ChildAgentInfoModel> GetChildAgentInfosAsync(
        GetChildAgentInfosRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var response = await _client
            .GetAsync($"/api/agent/allChildAgents/{request.AgentId}", cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        var playerCountInfos = JsonSerializer.Deserialize<ChildAgentInfoModel[]>(contentJson, _jsonSerializerOptions);

        foreach (var playerCountInfo in playerCountInfos!)
            yield return playerCountInfo;
    }

    public async ValueTask SetAgentPasswordAsync(SetAgentPasswordRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _client
            .PatchAsJsonAsync($"/api/agent/{request.AgentId}:password", request, _jsonSerializerOptions, cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();
    }

    public async ValueTask SetAgentInfoAsync(SetAgentInfoRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _client
            .PatchAsJsonAsync($"/api/agent/{request.AgentId}:info", request, _jsonSerializerOptions, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();
    }
}