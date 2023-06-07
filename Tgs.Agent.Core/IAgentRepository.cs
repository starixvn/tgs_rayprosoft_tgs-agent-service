using Tgs.Agent.Core.Models;

namespace Tgs.Agent.Core;

public interface IAgentRepository
{
    public ValueTask<AgentInfo?> GetAgentByIdAsync(Guid id, CancellationToken cancellationToken = default);

    public ValueTask<AgentInfo?> GetAgentByUsernameAsync(Guid brandId, string username, CancellationToken cancellationToken = default);

    public IAsyncEnumerable<ChildAgentInfoModel> GetChildAgentInfosAsync(Guid agentId, CancellationToken cancellationToken = default);

    public ValueTask SetAgentInfoAsync(Guid agentId, string? realName, string? email, string? phone, CancellationToken cancellationToken = default);

    public ValueTask SetPasswordAsync(Guid agentId, byte[] password, byte[] salt, CancellationToken cancellationToken = default);
}