using Microsoft.EntityFrameworkCore;
using Tgs.Agent.Core.Models;
using Tgs.Agent.Repositories.Entities;

namespace Tgs.Agent.Repositories;

internal class AgentRepository : IAgentRepository
{
    private readonly TgsContext _tgsContext;

    public AgentRepository(TgsContext tgsContext)
    {
        _tgsContext = tgsContext ?? throw new ArgumentNullException(nameof(tgsContext));
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

        var playerNumber = agent.Players.Count(p => p.AgentId == id);

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
                ParentAgentId = x.ParentAgentId!.Value
            })
            .ToAsyncEnumerable();

    public async ValueTask SetAgentInfoAsync(Guid agentId, string? realName, string? email, string? phone, CancellationToken cancellationToken = default)
    {
        var agent = await _tgsContext.Agents
            .FindAsync(new object?[] { agentId }, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (agent is null)
            return;

        agent.RealName = realName;
        agent.Email = email;
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
}