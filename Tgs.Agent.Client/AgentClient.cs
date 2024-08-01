using System.Collections.Immutable;
using System.Net;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using Tgs.Agent.Client.Models;
using Tgs.Agent.Client.Models.Enums;

namespace Tgs.Agent.Client;

public class AgentClient : IAgentClient
{
    private readonly HttpClient _client;

    private static readonly JsonSerializerOptions _JsonSerializerOptions = new()
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
           .PostAsJsonAsync("/api/auditlog", request, _JsonSerializerOptions, cancellationToken)
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

        return JsonSerializer.Deserialize<AgentInfoModel>(contentJson, _JsonSerializerOptions);
    }

    public async ValueTask<AgentInfoModel?> GetAgentInfoByUsernameAsync(GetAgentInfoByUsernameRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _client
            .GetAsync($"/api/agent/info/{request.BrandId}/{request.Username}", cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return string.IsNullOrEmpty(contentJson) ? null : JsonSerializer.Deserialize<AgentInfoModel>(contentJson, _JsonSerializerOptions);
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

        var playerCountInfos = JsonSerializer.Deserialize<ChildAgentInfoModel[]>(contentJson, _JsonSerializerOptions);

        foreach (var playerCountInfo in playerCountInfos!)
            yield return playerCountInfo;
    }

    public async ValueTask SetAgentPasswordAsync(SetAgentPasswordRequest request, CancellationToken cancellationToken = default)
    {
        var requestContent = new StringContent(
            JsonSerializer.Serialize(new { request.Password }, _JsonSerializerOptions),
            Encoding.UTF8,
            MediaTypeNames.Application.Json);

        if (!string.IsNullOrWhiteSpace(request.ModifyUserId))
            requestContent.Headers.Add("ModifyUserId", request.ModifyUserId);

        var response = await _client
            .PatchAsync($"/api/agent/{request.AgentId}:password", requestContent, cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();
    }

    public async ValueTask SetAgentDividendPercentageAsync(SetAgentDividendPercentageRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _client
            .PatchAsJsonAsync($"/api/agent/{request.AgentId}:dividend", request, _JsonSerializerOptions, cancellationToken)
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
            .PatchAsJsonAsync($"/api/agent/{request.AgentId}:info", request, _JsonSerializerOptions, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();
    }

    public async ValueTask SetAgentContactAsync(SetAgentContactRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _client
            .PatchAsJsonAsync($"/api/agent/{request.AgentId}:contact", request, _JsonSerializerOptions, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();
    }

    public async ValueTask SetAgentPayPasswordAsync(SetAgentPayPasswordRequest request, CancellationToken cancellationToken = default)
    {
        var requestContent = new StringContent(
            JsonSerializer.Serialize(new { request.PayPassword }, _JsonSerializerOptions),
            Encoding.UTF8,
            MediaTypeNames.Application.Json);

        if (!string.IsNullOrWhiteSpace(request.ModifyUserId))
            requestContent.Headers.Add("ModifyUserId", request.ModifyUserId);

        var response = await _client
            .PatchAsync($"/api/agent/{request.AgentId}:pay-password", requestContent, cancellationToken)
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

        return JsonSerializer.Deserialize<PagingResponse<AgentAuditLogInfo>>(contentJson, _JsonSerializerOptions)!;
    }

    public async ValueTask SetAgentPropertiesAsync(
        Guid agentId,
        IImmutableDictionary<AgentPropertyName, string?> properties,
        string? modifyUserId = null,
        CancellationToken cancellationToken = default)
    {
        var requestContent = new StringContent(
            JsonSerializer.Serialize(properties, _JsonSerializerOptions),
            Encoding.UTF8,
            MediaTypeNames.Application.Json);

        if (!string.IsNullOrWhiteSpace(modifyUserId))
            requestContent.Headers.Add("ModifyUserId", modifyUserId);

        var response = await _client
            .PostAsync($"/api/agent/prop/{agentId}", requestContent, cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();
    }

    public async ValueTask SetAgentStatusAsync(
        Guid agentId,
        AgentStatus agentStatus,
        string? modifyUserId = null,
        CancellationToken cancellationToken = default)
    {
        var requestContent = new StringContent(
            JsonSerializer.Serialize(agentStatus, _JsonSerializerOptions),
            Encoding.UTF8,
            MediaTypeNames.Application.Json);

        if (!string.IsNullOrWhiteSpace(modifyUserId))
            requestContent.Headers.Add("ModifyUserId", modifyUserId);

        var response = await _client
            .PutAsync($"/api/agent/status/{agentId}", requestContent, cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();
    }

    public IAsyncEnumerable<AgentHistoryInfo> GetAgentHistoriesAsync(
        GetAgentHistoriesQueryConditions conditions,
        int totalCount,
        int skip = 0,
        CancellationToken cancellationToken = default)
    {
        var queryStrings = HttpUtility.ParseQueryString(string.Empty);

        queryStrings.Add("from", conditions.From.ToString("o"));
        queryStrings.Add("to", conditions.To.ToString("o"));

        var remainingCount = totalCount - skip;
        var pages = (int)Math.Ceiling(remainingCount / 1000D);

        return AsyncEnumerableEx.Return(totalCount)
            .SelectMany(count => AsyncEnumerable.Range(0, pages)
                .SelectMany(idx => _client.GetAsync($"/api/agent/histories/{conditions.AgentId}?{queryStrings}&skip={skip + idx * 1000}")
                    .ToAsyncEnumerable()
                    .Do(response => response.EnsureSuccessStatusCode())
                    .SelectMany(response => response.Content
                        .ReadFromJsonAsync<IAsyncEnumerable<AgentHistoryInfo>>(_JsonSerializerOptions, cancellationToken)
                        .ToAsyncEnumerable()
                        .Where(arr => arr is not null)
                        .SelectMany(arr => arr!))))
            .Select(record => record);
    }

    public ValueTask<int> CountAgentHistoriesAsync(GetAgentHistoriesQueryConditions conditions, CancellationToken cancellationToken = default)
    {
        var queryStrings = HttpUtility.ParseQueryString(string.Empty);

        queryStrings.Add("from", conditions.From.ToString("o"));
        queryStrings.Add("to", conditions.To.ToString("o"));

        var requestPath = $"/api/agent/histories/{conditions.AgentId}?{queryStrings}";
        var countRequest = new HttpRequestMessage(HttpMethod.Head, $"{requestPath}");

        return _client.SendAsync(countRequest, cancellationToken)
            .ToAsyncEnumerable()
            .Do(response => response.EnsureSuccessStatusCode())
            .SelectMany(response => response.Headers
                .GetValues("Tgs-Return")
                .ToAsyncEnumerable()
                .Take(1))
            .Select(int.Parse)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async ValueTask<AgentEventInfo?> GetAgentHistoryAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        var response = await _client
           .GetAsync($"/api/agent/history/{eventId}", cancellationToken)
           .ConfigureAwait(false);

        switch (response.StatusCode)
        {
            case HttpStatusCode.OK:
                {
                    var content = await response.Content
                        .ReadAsStringAsync(cancellationToken)
                        .ConfigureAwait(false);

                    return JsonSerializer.Deserialize<AgentEventInfo>(content, _JsonSerializerOptions);
                }
            case HttpStatusCode.NotFound:
                return null;

            default:
                throw new AgentServiceApiException("Unknown error");
        }
    }

    public async ValueTask ClearAgentDividendConditionsAsync(Guid agentId, CancellationToken cancellationToken = default)
    {
        var response = await _client
            .DeleteAsync($"/api/agentdividendcondition/{agentId}", cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();
    }

    public async ValueTask UpdateSubAgentsPathAsync(Guid agentId, Guid newParentAgentId, CancellationToken cancellationToken = default)
    {
        var response = await _client
           .PatchAsync($"/api/agent/{agentId}:sub-agents/path/{newParentAgentId}", null, cancellationToken)
           .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();
    }

    public async ValueTask<Guid> CreateAgentAsync(CreateAgentRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _client
            .PostAsJsonAsync("/api/agent", request, _JsonSerializerOptions, cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<Guid>(contentJson, _JsonSerializerOptions);
    }

    public async ValueTask<Guid> CreateAgentByPlayerUpgradeAsync(CreateAgentByPlayerUpgradeRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _client
            .PostAsJsonAsync("/api/agent/create-by-player-upgrade", request, _JsonSerializerOptions, cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return JsonSerializer.Deserialize<Guid>(contentJson, _JsonSerializerOptions);
    }

    public async ValueTask SetAgentNegativeAgentDividendCarryForwardAsync(Guid agentId,
        bool negativeAgentDividendCarryForward,
        string? modifyUserId = null,
        CancellationToken cancellationToken = default)
    {
        var requestContent = new StringContent(
            JsonSerializer.Serialize(negativeAgentDividendCarryForward, _JsonSerializerOptions),
            Encoding.UTF8,
            MediaTypeNames.Application.Json);

        if (!string.IsNullOrWhiteSpace(modifyUserId))
            requestContent.Headers.Add("ModifyUserId", modifyUserId);

        var response = await _client
            .PutAsync($"/api/agent/negative-agent-dividend-carry-forward/{agentId}", requestContent, cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();
    }

    public async ValueTask SetOneLayerAgentInfoAsync(SetOneLayerAgentInfoRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _client
            .PatchAsJsonAsync($"/api/agent/{request.AgentId}/one-layer",
                request,
                _JsonSerializerOptions,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();
    }

    public async ValueTask SetMultipleLayerAgentInfoAsync(SetMultipleLayerAgentInfoRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _client
            .PatchAsJsonAsync($"/api/agent/{request.AgentId}/multiple-layer",
                request,
                _JsonSerializerOptions,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();
    }

    public async ValueTask SetAgentProductFeeAsync(SetAgentProductFeeRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _client
            .PutAsJsonAsync($"/api/agent/product-fee/{request.AgentId}",
                request,
                _JsonSerializerOptions,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();
    }

    public async ValueTask<AgentProductFeeInfo?> GetAgentProductFeeAsync(Guid agentId,
        CancellationToken cancellationToken = default)
    {
        var response = await _client
            .GetAsync($"/api/agent/product-fee/{agentId}", cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var contentJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        return string.IsNullOrEmpty(contentJson) ? null : JsonSerializer.Deserialize<AgentProductFeeInfo>(contentJson, _JsonSerializerOptions);
    }

    public async IAsyncEnumerable<AgentNameInfo> GetAgentNamesAsync(Guid brandId)
    {
        using var response = await _client.GetAsync($"/api/agent/{brandId}/agent-names", HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

        var infos = JsonSerializer.DeserializeAsyncEnumerable<AgentNameInfo>(
            responseStream,
            _JsonSerializerOptions);

        await foreach (var info in infos)
        {
            yield return info!;
        }
    }
}