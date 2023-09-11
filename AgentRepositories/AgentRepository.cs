using Microsoft.EntityFrameworkCore;
using Tgs.Agent.Core;
using Tgs.Agent.Core.Models;
using Tgs.Agent.Core.Models.Enums;
using Tgs.Agent.Repositories.Entities;
using TGS.Core.Infrastructures.Extensions;

namespace Tgs.Agent.Repositories;

internal class AgentRepository : IAgentRepository
{
    private readonly TgsContext _tgsContext;

    public AgentRepository(TgsContext tgsContext)
    {
        _tgsContext = tgsContext ?? throw new ArgumentNullException(nameof(tgsContext));
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
            agent.Status,
            agent.Type,
            agent.LevelType,
            agent.DividendPercentage,
            playerNumber,
            dividend);
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
            agent.Status,
            agent.Type,
            agent.LevelType,
            agent.DividendPercentage,
            playerNumber,
            dividend);
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

        _tgsContext.UpdateRange(commissions.ToEnumerable(), cancellationToken);

        _ = await _tgsContext.SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}