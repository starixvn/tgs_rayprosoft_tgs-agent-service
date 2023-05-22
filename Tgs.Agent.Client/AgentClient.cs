using System.Text.Json;
using Tgs.Agent.Client.Models;

namespace Tgs.Agent.Client;

public class AgentClient : IAgentClient
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

    public AgentClient(HttpClient client)
    {
        _client = client;
    }

    public async IAsyncEnumerable<ChildAgentInfoModel> GetChildAgentInfosAsync(GetChildAgentInfosRequest request)
    {
        var response = await _client
            .GetAsync($"/api/agent/allChildAgents/{request.AgentId}")
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

        await foreach (var playerCountInfo in JsonSerializer.DeserializeAsyncEnumerable<ChildAgentInfoModel>(
                           contentStream,
                           _jsonSerializerOptions)) yield return playerCountInfo!;
    }
}