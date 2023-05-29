using Tgs.Agent.Core.Models;
using Tgs.Agent.Models;

namespace Tgs.Agent;

public interface IAgentRepository
{
    public ValueTask<AgentInfo?> GetAgentByIdAsync(Guid id, CancellationToken cancellationToken = default);

    public IAsyncEnumerable<ChildAgentInfoModel> GetChildAgentInfosAsync(Guid agentId, CancellationToken cancellationToken = default);

    public ValueTask SetAgentInfoAsync(Guid agentId, string? realName, string? email, string? phone, CancellationToken cancellationToken = default);

    public ValueTask SetPasswordAsync(Guid agentId, byte[] newPassword, byte[] newSalt, CancellationToken cancellationToken = default);
}