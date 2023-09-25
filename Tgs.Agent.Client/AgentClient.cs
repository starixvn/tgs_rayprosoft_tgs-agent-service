using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using Tgs.Agent.Client.Models;

namespace Tgs.Agent.Client;

public class AgentClient : IAgentClient
{
    private readonly HttpClient _client;

    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        Converters = {
            new JsonStringEnumConverter()
        }
    };

    public AgentClient(HttpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask AddAgentAuditLogAsync(AddAgentAuditLogRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _client
           .PostAsJsonAsync("/api/auditlog", request, _jsonSerializerOptions, cancellationToken)
           .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();
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

    public async ValueTask<AgentInfoModel?> GetAgentInfoByUsernameAsync(GetAgentInfoByUsernameRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _client
            .GetAsync($"/api/agent/info/{request.BrandId}/{request.Username}", cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return string.IsNullOrEmpty(contentJson) ? null : JsonSerializer.Deserialize<AgentInfoModel>(contentJson, _jsonSerializerOptions);
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

    public async ValueTask SetAgentDividendPercentageAsync(SetAgentDividendPercentageRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _client
            .PatchAsJsonAsync($"/api/agent/{request.AgentId}:dividend", request, _jsonSerializerOptions, cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();
    }

    public async ValueTask ClearAgentNegativeCommissionAsync(ClearAgentNegativeCommissionRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _client
            .PutAsync($"/api/agentCommission/clear/{request.AgentId}/{request.ClearByUserId}", null, cancellationToken)
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

    public async ValueTask SetAgentContactAsync(SetAgentContactRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _client
            .PatchAsJsonAsync($"/api/agent/{request.AgentId}:contact", request, _jsonSerializerOptions, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();
    }

    public async ValueTask SetAgentPayPasswordAsync(SetAgentPayPasswordRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _client
            .PatchAsJsonAsync($"/api/agent/{request.AgentId}:pay-password", request, _jsonSerializerOptions, cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();
    }

    public async ValueTask<PagingResponse<AgentAuditLogInfo>> GetAgentAuditLogsAsync(
        GetAgentAuditLogQueryConditions conditions,
        Paging paging,
        CancellationToken cancellationToken = default)
    {
        var queryStrings = HttpUtility.ParseQueryString(string.Empty);

        queryStrings.Add("brand_id", conditions.BrandId.ToString());
        queryStrings.Add("agent_id", conditions.AgentId.ToString());
        queryStrings.Add("from", conditions.From.ToString("o"));
        queryStrings.Add("to", conditions.To.ToString("o"));

        if (paging.ShowAll == true)
            queryStrings.Add("all", "true");
        else
        {
            if (paging.Page != null)
                queryStrings.Add("page", paging.Page.ToString());

            if (paging.Rows != null)
                queryStrings.Add("rows", paging.Rows.ToString());
        }

        if (!string.IsNullOrWhiteSpace(paging.SortBy))
        {
            queryStrings.Add("sort", paging.SortBy);

            if (paging.IsDesc == true)
                queryStrings.Add("desc", "true");
        }

        var response = await _client
            .GetAsync($"/api/auditlog/{conditions.BrandId}?{queryStrings}", cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content
            .ReadAsStringAsync(cancellationToken)
            .ConfigureAwait(false);

        return JsonSerializer.Deserialize<PagingResponse<AgentAuditLogInfo>>(contentJson, _jsonSerializerOptions)!;
    }
}