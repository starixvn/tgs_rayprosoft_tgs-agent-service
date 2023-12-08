using System.Collections.Immutable;
using Tgs.Agent.Client.Models;
using Tgs.Agent.Client.Models.Enums;

namespace Tgs.Agent.Client;

public interface IAgentClient
{
    ValueTask<PagingResponse<AgentAuditLogInfo>> GetAgentAuditLogsAsync(GetAgentAuditLogQueryConditions conditions, Paging paging, CancellationToken cancellationToken = default);

    ValueTask AddAgentAuditLogAsync(AddAgentAuditLogRequest request, CancellationToken cancellationToken = default);

    ValueTask<AgentInfoModel?> GetAgentInfoAsync(GetAgentInfoRequest request, CancellationToken cancellationToken = default);

    ValueTask<AgentInfoModel?> GetAgentInfoByUsernameAsync(GetAgentInfoByUsernameRequest request, CancellationToken cancellationToken = default);

    IAsyncEnumerable<ChildAgentInfoModel> GetChildAgentInfosAsync(GetChildAgentInfosRequest request, CancellationToken cancellationToken = default);

    ValueTask SetAgentInfoAsync(SetAgentInfoRequest request, CancellationToken cancellationToken = default);

    ValueTask SetAgentContactAsync(SetAgentContactRequest request, CancellationToken cancellationToken = default);

    ValueTask SetAgentDividendPercentageAsync(SetAgentDividendPercentageRequest request, CancellationToken cancellationToken = default);

    ValueTask SetAgentPasswordAsync(SetAgentPasswordRequest request, CancellationToken cancellationToken = default);

    ValueTask ClearAgentNegativeCommissionAsync(ClearAgentNegativeCommissionRequest request, CancellationToken cancellationToken = default);

    ValueTask SetAgentPayPasswordAsync(SetAgentPayPasswordRequest request, CancellationToken cancellationToken = default);

    ValueTask SetAgentPropertiesAsync(
        Guid agentId,
        IImmutableDictionary<AgentPropertyName, string?> properties,
        string? modifyUserId = null,
        CancellationToken cancellationToken = default);

    ValueTask SetAgentStatusAsync(
        Guid agentId,
        AgentStatus agentStatus,
        string? modifyUserId = null,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<AgentHistoryInfo> GetAgentHistoriesAsync(
        GetAgentHistoriesQueryConditions conditions,
        int totalCount,
        int skip = 0,
        CancellationToken cancellationToken = default);

    ValueTask<int> CountAgentHistoriesAsync(
        GetAgentHistoriesQueryConditions conditions,
        CancellationToken cancellationToken = default);

    ValueTask<AgentEventInfo?> GetAgentHistoryAsync(Guid eventId, CancellationToken cancellationToken = default);

    ValueTask ClearAgentDividendConditionsAsync(Guid agentId, CancellationToken cancellationToken = default);

    ValueTask UpdateSubAgentsPathAsync(
        Guid agentId,
        Guid newParentAgentId,
        CancellationToken cancellationToken = default);

    ValueTask<Guid> CreateAgentAsync(
        CreateAgentRequest request,
        CancellationToken cancellationToken = default);
}