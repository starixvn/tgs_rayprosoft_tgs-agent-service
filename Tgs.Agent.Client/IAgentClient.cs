using Tgs.Agent.Client.Models;

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
}