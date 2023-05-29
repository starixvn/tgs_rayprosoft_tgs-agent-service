using Tgs.Agent.Core.Models;
using Tgs.Agent.Models;
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

        return new AgentInfo(
               agent.Id,
               ParentAgentId: agent.ParentAgentId,
               agent.RealName,
               agent.Phone,
               agent.Email);
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

        agent!.RealName = realName;
        agent.Email = email;
        agent.Phone = phone;
        agent.UpdatedOn = DateTime.UtcNow;

        _ = await _tgsContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask SetPasswordAsync(Guid agentId, byte[] newPassword, byte[] newSalt, CancellationToken cancellationToken = default)
    {
        var agent = await _tgsContext.Agents
            .FindAsync(new object?[] { agentId }, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        agent!.Password = newPassword;
        agent.Salt = newSalt;
        agent.UpdatedOn = DateTime.UtcNow;

        _ = await _tgsContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}