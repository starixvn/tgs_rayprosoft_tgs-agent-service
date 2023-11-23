using System.Text.Json;
using Tgs.Agent.Core.Models;
using Tgs.Agent.Core.Models.Enums;
using Tgs.Paging;

namespace Tgs.Agent.Core;

public interface IAgentRepository
{
    IAsyncEnumerable<AgentAuditLogInfo> QueryAgentAuditLogsAsync(AgenAuditLogQueryConditions conditions, Paging<AgentAuditLogInfo> paging, CancellationToken cancellationToken = default);

    ValueTask<int> CountAgentAuditLogsAsync(AgenAuditLogQueryConditions conditions, CancellationToken cancellationToken = default);

    public ValueTask AddAgentAuditLogAsync(Guid agentId, AgentOperation operation, string editUsername, CancellationToken cancellationToken = default);

    public ValueTask<AgentInfo?> GetAgentByIdAsync(Guid id, CancellationToken cancellationToken = default);

    public ValueTask<AgentInfo?> GetAgentByUsernameAsync(Guid brandId, string username, CancellationToken cancellationToken = default);

    public IAsyncEnumerable<ChildAgentInfoModel> GetChildAgentInfosAsync(Guid agentId, CancellationToken cancellationToken = default);

    public ValueTask SetAgentDividendPercentageAsync(Guid agentId, decimal dividendPercentage, CancellationToken cancellationToken = default);

    public ValueTask SetAgentInfoAsync(Guid agentId, string? realName, CancellationToken cancellationToken = default);

    public ValueTask SetAgentContactAsync(Guid agentId, string? email, string? phone, CancellationToken cancellationToken = default);

    public ValueTask SetPasswordAsync(Guid agentId, byte[] password, byte[] salt, CancellationToken cancellationToken = default);

    public Task ClearNegativeCommissionAsync(Guid agentId, Guid clearByUserId, CancellationToken cancellationToken = default);

    public ValueTask SetPayPasswordAsync(Guid agentId, byte[] payPassword, byte[] paySalt, CancellationToken cancellationToken = default);

    ValueTask<EventInfo?> GetAgentEventAsync(Guid eventId, CancellationToken cancellationToken = default);

    IAsyncEnumerable<AgentHistoryInfo> QueryAgentEventAsync(
        AgentEventQueryConditions conditions,
        int skip = 0,
        CancellationToken cancellationToken = default);

    ValueTask<int> CountAgentEventAsync(AgentEventQueryConditions conditions, CancellationToken cancellationToken = default);

    ValueTask UpdateAgentSnapshotAsync(
        Guid agentId,
        Guid snapshotId,
        DateTime updatedOn,
        CancellationToken cancellationToken = default);

    ValueTask CreateAgentEventAsync(
        Guid eventId,
        Guid brandId,
        Guid agentId,
        EventAction action,
        JsonDocument changeSets,
        string? modifyUserId,
        CancellationToken cancellationToken = default);

    ValueTask SetAgentStatusAsync(
        Guid agentId,
        AgentStatus status,
        CancellationToken cancellationToken = default);

    ValueTask ClearAgentDividendConditionsAsync(
        Guid agentId,
        CancellationToken cancellationToken = default);

    ValueTask<AgentPathInfo?> GetAgentPathInfoAsync(
        Guid agentId,
        CancellationToken cancellationToken = default);

    ValueTask UpdateSubAgentsPathAsync(
        Guid agentId,
        int agentNLevel,
        string agentPath,
        string newParentAgentPath,
        CancellationToken cancellationToken = default);

    ValueTask UpdateAgentSnapshotAndPropertiesAsync(
        Guid agentId,
        Guid brandId,
        Guid snapshotId,
        IEnumerable<IDataCommand> modifyCommands,
        DateTime updatedOn,
        CancellationToken cancellationToken = default);

    ValueTask UpdateAgentSnapshotAndStatusAsync(
        Guid agentId,
        AgentStatus status,
        Guid snapshotId,
        DateTime updatedOn,
        CancellationToken cancellationToken = default);
}