using System.Data;
using System.Text.Json;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Tgs.Agent.Core;
using Tgs.Agent.Core.Models;
using Tgs.Agent.Core.Models.Enums;
using Tgs.Agent.Repositories.Entities;
using Tgs.Paging;
using TGS.Core.Infrastructures.Extensions;

namespace Tgs.Agent.Repositories;

internal class AgentRepository : IAgentRepository
{
    private readonly TgsContext _tgsContext;
    private readonly ILogger<AgentRepository> _logger;

    public AgentRepository(
        TgsContext tgsContext,
        ILogger<AgentRepository> logger)
    {
        _tgsContext = tgsContext ?? throw new ArgumentNullException(nameof(tgsContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async ValueTask AddAgentAuditLogAsync(Guid agentId, AgentOperation operation, string editUsername, CancellationToken cancellationToken = default)
    {
        var auditLog = new AgentAuditLog
        {
            Id = Guid.NewGuid(),
            AgentId = agentId,
            CreatedByUsername = editUsername,
            CreatedOn = DateTime.UtcNow,
            Operation = (short)operation
        };

        _ = await _tgsContext.AgentAuditLogs
            .AddAsync(auditLog, cancellationToken)
            .ConfigureAwait(false);

        _ = await _tgsContext
            .SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async ValueTask<AgentInfo?> GetAgentByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var agent = await _tgsContext.Agents
            .FindAsync(new object?[] { id }, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (agent == null)
            return null;

        var dividend = await _tgsContext.AgentDividends
            .Where(x => x.AgentId == id && x.SentOn == null)
            .SumAsync(x => x.BeforeDividendAmount ?? x.AgentDividendAmount, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var playerNumber = await _tgsContext.Players
            .CountAsync(p => p.AgentId == id, cancellationToken)
            .ConfigureAwait(false);

        return new AgentInfo(
            agent.Id,
            agent.Username,
            agent.RealName,
            agent.Phone,
            agent.Email,
            agent.Qq,
            agent.Wechat,
            agent.InvitationCode,
            agent.ParentAgentId,
            new DateTimeOffset(agent.RegisteredOn),
            (AgentStatus)agent.Status,
            (AgentType)agent.Type,
            (AgentLevelType)agent.LevelType,
            agent.DividendPercentage,
            playerNumber,
            dividend,
            agent.BrandId,
            agent.DefaultWalletId,
            agent.WalletId);
    }

    public async ValueTask<AgentInfo?> GetAgentByUsernameAsync(Guid brandId, string username,
        CancellationToken cancellationToken = default)
    {
        var agent = await _tgsContext.Agents
            .FirstOrDefaultAsync(x => x.BrandId == brandId && x.Username == username, cancellationToken)
            .ConfigureAwait(false);

        if (agent == null)
            return null;

        var dividend = await _tgsContext.AgentDividends
            .Where(x => x.AgentId == agent.Id && x.SentOn == null)
            .SumAsync(x => x.BeforeDividendAmount ?? x.AgentDividendAmount, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var playerNumber = await _tgsContext.Players
            .CountAsync(p => p.AgentId == agent.Id, cancellationToken)
            .ConfigureAwait(false);

        return new AgentInfo(
            agent.Id,
            agent.Username,
            agent.RealName,
            agent.Phone,
            agent.Email,
            agent.Qq,
            agent.Wechat,
            agent.InvitationCode,
            agent.ParentAgentId,
            new DateTimeOffset(agent.RegisteredOn),
            (AgentStatus)agent.Status,
            (AgentType)agent.Type,
            (AgentLevelType)agent.LevelType,
            agent.DividendPercentage,
            playerNumber,
            dividend,
            agent.BrandId,
            agent.DefaultWalletId,
            agent.WalletId);
    }

    public IAsyncEnumerable<ChildAgentInfoModel> GetChildAgentInfosAsync(Guid agentId, CancellationToken cancellationToken = default)
        => _tgsContext.Agents
            .Where(x => x.Id != agentId)
            .Where(x => x.Path.MatchesLQuery($"*.{agentId:N}.*"))
            .Select(x => new ChildAgentInfoModel
            {
                AgentId = x.Id,
                AgentUsername = x.Username,
                ParentAgentId = x.ParentAgentId!.Value,
                DividendPercentage = x.DividendPercentage,
            })
            .ToAsyncEnumerable();

    public async ValueTask SetAgentDividendPercentageAsync(Guid agentId, decimal dividendPercentage, CancellationToken cancellationToken = default)
    {
        var agent = await _tgsContext.Agents
            .FindAsync(new object?[] { agentId }, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (agent is null)
            return;

        agent.DividendPercentage = dividendPercentage;
        agent.UpdatedOn = DateTime.UtcNow;

        _ = await _tgsContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask SetAgentInfoAsync(Guid agentId, string? realName, CancellationToken cancellationToken = default)
    {
        var agent = await _tgsContext.Agents
            .FindAsync(new object?[] { agentId }, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (agent is null)
            return;

        agent.RealName = realName;
        agent.UpdatedOn = DateTime.UtcNow;

        _ = await _tgsContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask SetAgentContactAsync(Guid agentId, string? email, string? phone, CancellationToken cancellationToken = default)
    {
        var agent = await _tgsContext.Agents
            .FindAsync(new object?[] { agentId }, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (agent is null)
            return;

        if (!string.IsNullOrWhiteSpace(email))
            agent.Email = email;
        if (!string.IsNullOrWhiteSpace(phone))
            agent.Phone = phone;
        agent.UpdatedOn = DateTime.UtcNow;

        _ = await _tgsContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask SetPasswordAsync(Guid agentId, byte[] password, byte[] salt, CancellationToken cancellationToken = default)
    {
        var agent = await _tgsContext.Agents
            .FindAsync(new object?[] { agentId }, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (agent is null)
            return;

        agent.Password = password;
        agent.Salt = salt;
        agent.UpdatedOn = DateTime.UtcNow;

        _ = await _tgsContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task ClearNegativeCommissionAsync(Guid agentId, Guid clearByUserId,
        CancellationToken cancellationToken = default)
    {
        var commissions = _tgsContext.AgentDividends
            .Where(x => x.AgentId == agentId && x.SentOn == null)
            .ToAsyncEnumerable();

        await _tgsContext.AddRangeAsync(
                commissions.Select(x => new AgentClearCommissionRecord
                {
                    Id = GuidExtensions.NewSequentialGuid(),
                    AgentDividendId = x.Id,
                    CommissionAmount = x.AgentDividendAmount,
                    ClearByUserId = clearByUserId,
                    CreatedOn = DateTime.UtcNow,
                })
                    .ToEnumerable(),
                cancellationToken)
            .ConfigureAwait(false);

        await commissions.ForEachAsync(x => x.SentOn = DateTime.UtcNow, cancellationToken)
            .ConfigureAwait(false);

        _tgsContext.AgentDividends.UpdateRange(commissions.ToEnumerable());

        _ = await _tgsContext.SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async ValueTask SetPayPasswordAsync(Guid agentId, byte[] payPassword, byte[] paySalt, CancellationToken cancellationToken = default)
    {
        var agent = await _tgsContext.Agents
            .FindAsync(new object?[] { agentId }, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (agent is null)
            return;

        agent.WithdrawPassword = payPassword;
        agent.WithdrawSalt = paySalt;
        agent.UpdatedOn = DateTime.UtcNow;

        _ = await _tgsContext
            .SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    private IQueryable<AgentAuditLog> MakeAgentAuditLogQuery(AgenAuditLogQueryConditions conditions)
    {
        var dataQuery = _tgsContext.AgentAuditLogs.AsQueryable()
            .Where(log => log.AgentId == conditions.AgentId
                && log.Agent.BrandId == conditions.BrandId
                && log.CreatedOn >= conditions.From.UtcDateTime
                && log.CreatedOn < conditions.To.UtcDateTime);

        return dataQuery;
    }

    public IAsyncEnumerable<AgentAuditLogInfo> QueryAgentAuditLogsAsync(
        AgenAuditLogQueryConditions conditions,
        Paging<AgentAuditLogInfo> paging,
        CancellationToken cancellationToken = default)
    {
        var dataQuery = MakeAgentAuditLogQuery(conditions)
            .Select(l => new AgentAuditLogInfo(
                l.Id,
                (AgentOperation)l.Operation,
                l.CreatedByUsername,
                l.CreatedOn))
            .ToAsyncEnumerable();

        dataQuery = dataQuery.ApplyOrderBy(paging);

        if (paging.RowCount > 0)
            dataQuery = dataQuery.ApplyPaging(paging);

        return dataQuery;
    }

    public async ValueTask<int> CountAgentAuditLogsAsync(AgenAuditLogQueryConditions conditions, CancellationToken cancellationToken = default)
    {
        var dataQuery = MakeAgentAuditLogQuery(conditions);

        return await dataQuery
            .CountAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async ValueTask<EventInfo?> GetAgentEventAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        var eventSource = await _tgsContext.AgentEventSources
            .FirstOrDefaultAsync(p => p.Id == eventId, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return eventSource is null
            ? null
            : new EventInfo(
                Enum.Parse<EventAction>(eventSource.Action),
                JsonDocument.Parse(eventSource.ChangeSets),
                eventSource.ModifyUser,
                eventSource.CreatedOn);
    }

    public IAsyncEnumerable<AgentHistoryInfo> QueryAgentEventAsync(AgentEventQueryConditions conditions, int skip = 0, CancellationToken cancellationToken = default)
    {
        var dataQuery = MakeAgentHistoriesQuery(conditions);

        return dataQuery
            .OrderByDescending(record => record.CreatedOn)
            .Select(record => new
            {
                record.Id,
                record.Action,
                record.ModifyUser,
                record.CreatedOn,
            })
            .Skip(skip)
            .Take(1000)
            .ToAsyncEnumerable()
            .Select(row => new AgentHistoryInfo(
                row.Id,
                Enum.Parse<EventAction>(row.Action),
                row.ModifyUser,
                row.CreatedOn));
    }

    public async ValueTask UpdateAgentSnapshotAsync(Guid agentId, Guid snapshotId, DateTime updatedOn, CancellationToken cancellationToken = default)
    {
        var dbSnapshot = await _tgsContext.Agents
            .FindAsync(new object?[] { agentId }, cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? (await _tgsContext.Agents.AddAsync(new Entities.Agent
            {
                Id = agentId
            }, cancellationToken).ConfigureAwait(false)).Entity;

        _logger.LogInformation("UpdateAgentSnapshotAsync agent remarks:{remarks}", dbSnapshot.Remarks);

        if (dbSnapshot.SnapshotEventId is null || dbSnapshot.SnapshotEventId.Value.CompareTo(snapshotId) == -1)
        {
            dbSnapshot.SnapshotEventId = snapshotId;
            dbSnapshot.UpdatedOn = updatedOn;
        }

        _ = await _tgsContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask CreateAgentEventAsync(
        Guid eventId,
        Guid brandId,
        Guid agentId,
        EventAction action,
        JsonDocument changeSets,
        string? modifyUserId,
        CancellationToken cancellationToken = default)
    {
        _ = await _tgsContext.AgentEventSources.AddAsync(
            new AgentEventSource
            {
                Id = eventId,
                BrandId = brandId,
                Action = action.ToString(),
                AgentId = agentId,
                ChangeSets = JsonSerializer.Serialize(changeSets),
                ModifyUser = modifyUserId,
                CreatedOn = DateTime.UtcNow,
            },
            cancellationToken).ConfigureAwait(false);

        _ = await _tgsContext
            .SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    private IQueryable<AgentEventSource> MakeAgentHistoriesQuery(AgentEventQueryConditions conditions)
    {
        var dataQuery =
            from p in _tgsContext.AgentEventSources.AsQueryable()
            where p.AgentId == conditions.AgentId
                && p.CreatedOn >= conditions.From
                && p.CreatedOn < conditions.To
            select p;

        return dataQuery;
    }

    public async ValueTask<int> CountAgentEventAsync(AgentEventQueryConditions conditions, CancellationToken cancellationToken = default)
        => await MakeAgentHistoriesQuery(conditions)
            .CountAsync(cancellationToken)
            .ConfigureAwait(false);

    public async ValueTask SetAgentPropertiesAsync(Guid brandId, Guid agentId, IEnumerable<IDataCommand> modifyCommands, CancellationToken cancellationToken = default)
    {
        var agent = await _tgsContext.Agents
            .FindAsync(new object?[]
            {
                agentId
            }, cancellationToken)
            .ConfigureAwait(false);

        if (agent == null)
            return;

        var context = new AgentModifyCommandContext(
            agent,
            _tgsContext);

        foreach (var command in modifyCommands)
            await command
                .ExecuteAsync(context, cancellationToken)
                .ConfigureAwait(false);

        agent.UpdatedOn = DateTime.UtcNow;

        _ = await _tgsContext
            .SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async ValueTask SetAgentStatusAsync(Guid agentId, AgentStatus status, CancellationToken cancellationToken = default)
    {
        var agent = await _tgsContext.Agents
            .FindAsync(new object?[]
            {
                agentId
            }, cancellationToken)
            .ConfigureAwait(false);

        if (agent == null)
            return;

        agent.Status = (short)status;
        agent.UpdatedOn = DateTime.UtcNow;

        _ = await _tgsContext
            .SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async ValueTask ClearAgentDividendConditionsAsync(Guid agentId, CancellationToken cancellationToken = default)
    {
        var agentDividendConditions = _tgsContext.AgentDividendConditions
            .Where(x => x.AgentId == agentId)
            .ToAsyncEnumerable();

        _tgsContext.AgentDividendConditions.RemoveRange(agentDividendConditions.ToEnumerable());

        _ = await _tgsContext
            .SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async ValueTask<AgentPathInfo?> GetAgentPathInfoAsync(Guid agentId, CancellationToken cancellationToken = default)
    {
        var query = from a in _tgsContext.Agents.AsQueryable()
                    where a.Id == agentId
                    select new { a.Id, a.Path, a.Path.NLevel };
        var agent = await query
            .SingleOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        if (agent == null)
            return null;

        return new AgentPathInfo(agent.Id, agent.NLevel, agent.Path.ToString());
    }

    public async ValueTask UpdateSubAgentsPathAsync(
        Guid agentId,
        int agentNLevel,
        string agentPath,
        string newParentAgentPath,
        CancellationToken cancellationToken = default)
    {
        var sql = "SELECT * from dbo.agent_update_path(@agentId,@agentNlevel,@agentPah,@newAgentPath)";

        var dbConnection = _tgsContext.Database.GetDbConnection();
        var transaction = _tgsContext.Database.CurrentTransaction?.GetDbTransaction();

        var parameters = new Dictionary<string, object>
        {
            { "@agentId", agentId },
            { "@agentNlevel", agentNLevel },
            { "@agentPah", agentPath },
            { "@newAgentPath", newParentAgentPath }
        };

        await dbConnection.ExecuteAsync(
            sql,
            parameters,
            transaction,
            null,
            CommandType.Text).ConfigureAwait(false);
    }
}