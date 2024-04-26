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
using Tgs.Core.Infrastructures.Utility;
using Tgs.Paging;
using TGS.Core.Infrastructures.Extensions;

namespace Tgs.Agent.Repositories;

internal class AgentRepository(
    TgsContext tgsContext,
    ILogger<AgentRepository> logger)
    : IAgentRepository
{
    private readonly TgsContext _tgsContext = tgsContext ?? throw new ArgumentNullException(nameof(tgsContext));
    private readonly ILogger<AgentRepository> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

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
            agent.WalletId,
            agent.WithdrawPassword.Length > 0,
            agent.NegativeAgentDividendCarryForward);
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
            agent.WalletId,
            agent.WithdrawPassword.Length > 0,
            agent.NegativeAgentDividendCarryForward);
    }

    public IAsyncEnumerable<ChildAgentInfoModel> GetChildAgentInfosAsync(Guid agentId, CancellationToken cancellationToken = default)
        => _tgsContext.Agents
            .Where(x => x.Id != agentId)
            .Where(x => x.Path.MatchesLQuery($"*.{agentId:N}.*"))
            .Select(x => new ChildAgentInfoModel
            {
                AgentId = x.Id,
                AgentUsername = x.Username,
                RealName = x.RealName,
                Status = (AgentStatus)x.Status,
                ParentAgentId = x.ParentAgentId!.Value,
                DividendPercentage = x.DividendPercentage,
                RegisteredOn = x.RegisteredOn
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
            .Where(x => x.AgentId == agentId &&
                        x.SentOn == null &&
                        x.ReferencedAgentDividendId == null &&
                        x.BeforeDividendAmount == null)
            .ToAsyncEnumerable();

        var total = await commissions.SumAsync(x => x.AgentDividendAmount, cancellationToken);

        if(total >= 0)
            return;

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

    public async ValueTask UpdateSubAgentsPathAsync(AgentPathInfo agentPathInfo,
        string newParentAgentPath,
        CancellationToken cancellationToken = default)
    {
        var sql = "SELECT * from dbo.agent_update_path(@agentId,@agentNlevel,@agentPah,@newAgentPath)";

        var dbConnection = _tgsContext.Database.GetDbConnection();
        var transaction = _tgsContext.Database.CurrentTransaction?.GetDbTransaction();

        var parameters = new Dictionary<string, object>
        {
            { "@agentId", agentPathInfo.AgentId },
            { "@agentNlevel", agentPathInfo.NLevel },
            { "@agentPah", agentPathInfo.Path },
            { "@newAgentPath", newParentAgentPath }
        };

        await dbConnection.ExecuteAsync(
            sql,
            parameters,
            transaction,
            null,
            CommandType.Text).ConfigureAwait(false);
    }

    public async ValueTask UpdateAgentSnapshotAndPropertiesAsync(
        Guid agentId,
        Guid brandId,
        Guid snapshotId,
        IEnumerable<IDataCommand> modifyCommands,
        DateTime updatedOn,
        CancellationToken cancellationToken = default)
    {
        var dbSnapshot = await _tgsContext.Agents
            .FindAsync(new object?[] { agentId }, cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? (await _tgsContext.Agents.AddAsync(new Entities.Agent
            {
                Id = agentId
            }, cancellationToken).ConfigureAwait(false)).Entity;

        if (dbSnapshot.SnapshotEventId is null || dbSnapshot.SnapshotEventId.Value.CompareTo(snapshotId) == -1)
        {
            dbSnapshot.SnapshotEventId = snapshotId;
            dbSnapshot.UpdatedOn = updatedOn;
        }

        var context = new AgentModifyCommandContext(dbSnapshot, _tgsContext);

        foreach (var command in modifyCommands)
            await command
                .ExecuteAsync(context, cancellationToken)
                .ConfigureAwait(false);

        _ = await _tgsContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask UpdateAgentSnapshotAndStatusAsync(
        Guid agentId,
        AgentStatus status,
        Guid snapshotId,
        DateTime updatedOn,
        CancellationToken cancellationToken = default)
    {
        var dbSnapshot = await _tgsContext.Agents
            .FindAsync(new object?[] { agentId }, cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? (await _tgsContext.Agents.AddAsync(new Entities.Agent
            {
                Id = agentId
            }, cancellationToken).ConfigureAwait(false)).Entity;

        if (dbSnapshot.SnapshotEventId is null || dbSnapshot.SnapshotEventId.Value.CompareTo(snapshotId) == -1)
        {
            dbSnapshot.SnapshotEventId = snapshotId;
            dbSnapshot.UpdatedOn = updatedOn;
        }

        dbSnapshot.Status = (short)status;
        _ = await _tgsContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask UpdateAgentNegativeAgentDividendCarryForwardAsync(Guid agentId,
        bool negativeAgentDividendCarryForward,
        CancellationToken cancellationToken = default)
    {
        var agent = await _tgsContext.Agents.FirstAsync(x => x.Id == agentId, cancellationToken).ConfigureAwait(false);

        agent.NegativeAgentDividendCarryForward = negativeAgentDividendCarryForward;

        _ = await _tgsContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<Guid?> CheckAgentPlayerRelationExistsAsync(string playerCode,
        CancellationToken cancellationToken = default)
    {
        var playerAgentRelation = await _tgsContext.PlayerAgentRelations
            .FirstOrDefaultAsync(x => x.PlayerCode == playerCode, cancellationToken)
            .ConfigureAwait(false);

        return playerAgentRelation?.AgentId;
    }

    public async Task CreateAgentAsync(AgentModel agentModel, CancellationToken cancellationToken = default)
    {
        var invitationCode = InvitationCodeGenerator.NewId();

        while (await _tgsContext.Agents.AnyAsync(x => x.InvitationCode == invitationCode, cancellationToken)
                   .ConfigureAwait(false))
            invitationCode = InvitationCodeGenerator.NewId();

        var parentAgent =
            await _tgsContext.Agents.FirstOrDefaultAsync(x => x.Id == agentModel.ParentAgentId, cancellationToken)
                .ConfigureAwait(false);

        var brand = await _tgsContext.Brands.Include(x => x.BrandAgentDividendConditions)
            .FirstAsync(x => x.Id == agentModel.BrandId, cancellationToken)
            .ConfigureAwait(false);

        var agent = new Entities.Agent
        {
            Id = agentModel.AgentId,
            Username = agentModel.Username,
            LevelType = (short)agentModel.LevelType,
            Type = (short)agentModel.Type,
            BrandId = agentModel.BrandId,
            Status = (short)AgentStatus.Enabled,
            ParentAgentId = agentModel.ParentAgentId,
            DefaultWalletId = agentModel.WalletId,
            WalletId = agentModel.WalletId,
            AgentStatus =
                brand is { AgentDividendType: (short)AgentDividendType.MultipleLayer, BrandAgentDividendConditions.Count: > 0 } ||
                brand.AgentDividendType == (short)AgentDividendType.OneLayer && agentModel.DividendPercentage > 0
                    ? (short)AgentConfigStatus.DoneForConfigurePercentage
                    : (short)AgentConfigStatus.WaitForConfigurePercentage,
            CommissionPercentage = agentModel.CommissionPercentage,
            DividendPercentage = agentModel.DividendPercentage,
            LiveDealerRebatePercentage = agentModel.LiveDealerRebatePercentage,
            SlotRebatePercentage = agentModel.SlotRebatePercentage,
            LotteryRebatePercentage = agentModel.LotteryRebatePercentage,
            CardGameRebatePercentage = agentModel.CardGameRebatePercentage,
            SportsRebatePercentage = agentModel.SportsRebatePercentage,
            VirtualSportsRebatePercentage = agentModel.VirtualSportsRebatePercentage,
            InvitationCode = invitationCode,
            Remarks = agentModel.Remarks,
            CreatedByUsername = agentModel.EditorUsername,
            UpdatedByUsername = agentModel.EditorUsername,
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow,
            RegisteredOn = DateTime.UtcNow,
            Path = agentModel.ParentAgentId == null ? new LTree($"{agentModel.AgentId:N}") : new LTree($"{parentAgent!.Path}.{agentModel.AgentId:N}"),
            Password = agentModel.Password,
            Salt = agentModel.Salt,
            WithdrawPassword = Array.Empty<byte>(),
            WithdrawSalt = Array.Empty<byte>(),
            RealName = agentModel.RealName,
            Email = agentModel.Email,
            Phone = agentModel.Phone,
            NegativeAgentDividendCarryForward = agentModel.NegativeAgentDividendCarryForward
        };

        _ = await _tgsContext.Agents
            .AddAsync(agent, cancellationToken)
            .ConfigureAwait(false);

        _ = await _tgsContext
            .SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task UpdateAgentInfoAsync(Guid agentId,
        LTree newAgentPath,
        Guid? agentParentAgentId,
        AgentLevelType agentLevelType,
        AgentType agentType,
        AgentConfigStatus agentStatus,
        decimal agentDividendPercentage,
        bool negativeAgentDividendCarryForward,
        CancellationToken cancellationToken = default)
    {
        _ = await _tgsContext.Agents.Where(x => x.Id == agentId)
            .ExecuteUpdateAsync(x => x.SetProperty(s => s.Path, newAgentPath)
                    .SetProperty(s => s.ParentAgentId, agentParentAgentId)
                    .SetProperty(s => s.LevelType, (short)agentLevelType)
                    .SetProperty(s => s.Type, (short)agentType)
                    .SetProperty(s => s.AgentStatus, (short)agentStatus)
                    .SetProperty(s => s.DividendPercentage, agentDividendPercentage)
                    .SetProperty(s => s.NegativeAgentDividendCarryForward, negativeAgentDividendCarryForward),
                cancellationToken);

        _ = await _tgsContext.SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task ResetSubAgentDividendPercentageAsync(Guid[] agentIds,
        CancellationToken cancellationToken = default)
    {
        _ = await _tgsContext.Agents.Where(x => agentIds.Contains(x.Id))
            .ExecuteUpdateAsync(x =>
                x.SetProperty(s => s.DividendPercentage, 0M)
                    .SetProperty(s => s.AgentStatus, (short)AgentConfigStatus.WaitForConfigurePercentage),
                cancellationToken);

        _ = await _tgsContext.SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task UpdateAgentMultipleLayerConditionsAsync(Guid agentId,
        AgentDividendConditionModel[] agentDividendConditions,
        CancellationToken cancellationToken = default)
    {
        _ = await _tgsContext.AgentDividendConditions.Where(x => x.AgentId == agentId)
            .ExecuteDeleteAsync(cancellationToken)
            .ConfigureAwait(false);

        await _tgsContext.AgentDividendConditions.AddRangeAsync(agentDividendConditions.Select(x => new AgentDividendCondition
            {
                Rank = x.Rank,
                NetProfit = x.NetProfit,
                ActiveNumber = x.ActiveNumber,
                DividendPercentage = x.DividendPercentage
            }),
            cancellationToken)
            .ConfigureAwait(false);

        _ = await _tgsContext.SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task CreateAgentProductFeeAsync(Guid agentId,
        decimal cardGamePercentage,
        decimal liveDealerPercentage,
        decimal lotteryPercentage,
        decimal slotPercentage,
        decimal sportsPercentage,
        decimal virtualSportsPercentage,
        string editorUsername,
        CancellationToken cancellationToken = default)
    {
        _ = await _tgsContext.AgentProductFees.AddAsync(new AgentProductFee
            {
                AgentId = agentId,
                CardGamePercentage = cardGamePercentage,
                LiveDealerPercentage = liveDealerPercentage,
                LotteryPercentage = lotteryPercentage,
                SlotPercentage = slotPercentage,
                SportsPercentage = sportsPercentage,
                VirtualSportsPercentage = virtualSportsPercentage,
                CreatedByUsername = editorUsername,
                UpdatedByUsername = editorUsername,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow
            },
            cancellationToken);

        _ = await _tgsContext.SaveChangesAsync(cancellationToken);
    }


    public async Task UpdateAgentProductFeeAsync(Guid agentId,
        decimal cardGamePercentage,
        decimal liveDealerPercentage,
        decimal lotteryPercentage,
        decimal slotPercentage,
        decimal sportsPercentage,
        decimal virtualSportsPercentage,
        string editorUsername,
        CancellationToken cancellationToken = default)
    {
        var agent = await _tgsContext.AgentProductFees.FirstAsync(x => x.AgentId == agentId, cancellationToken);

        agent.CardGamePercentage = cardGamePercentage;
        agent.LiveDealerPercentage = liveDealerPercentage;
        agent.LotteryPercentage = lotteryPercentage;
        agent.SlotPercentage = slotPercentage;
        agent.SportsPercentage = sportsPercentage;
        agent.VirtualSportsPercentage = virtualSportsPercentage;
        agent.UpdatedByUsername = editorUsername;
        agent.UpdatedOn = DateTime.UtcNow;

        _ = _tgsContext.AgentProductFees.Update(agent);

        _ = await _tgsContext.SaveChangesAsync(cancellationToken);
    }

    public async ValueTask<AgentProductFeeInfo?> GetAgentProductFeeAsync(Guid agentId,
        CancellationToken cancellationToken = default)
    {
        var agent = await _tgsContext.AgentProductFees.FirstOrDefaultAsync(x => x.AgentId == agentId, cancellationToken);

        return agent == null
            ? null
            : new AgentProductFeeInfo(agent.AgentId,
                agent.CardGamePercentage,
                agent.LiveDealerPercentage,
                agent.LotteryPercentage,
                agent.SlotPercentage,
                agent.SportsPercentage,
                agent.VirtualSportsPercentage);
    }
}