using Tgs.Agent.Core.Models;
using Tgs.Agent.Core.Models.Enums;

namespace Tgs.Agent.Core;

public interface IAgentRepository
{
    public ValueTask AddAgentAuditLogAsync(Guid agentId, AgentOperation operation, string editUsername, CancellationToken cancellationToken = default);

    public ValueTask<AgentInfo?> GetAgentByIdAsync(Guid id, CancellationToken cancellationToken = default);

    public ValueTask<AgentInfo?> GetAgentByUsernameAsync(Guid brandId, string username, CancellationToken cancellationToken = default);

    public IAsyncEnumerable<ChildAgentInfoModel> GetChildAgentInfosAsync(Guid agentId, CancellationToken cancellationToken = default);

    public ValueTask SetAgentDividendPercentageAsync(Guid agentId, decimal dividendPercentage, CancellationToken cancellationToken = default);

    public ValueTask SetAgentInfoAsync(Guid agentId, string? realName, CancellationToken cancellationToken = default);

    public ValueTask SetAgentContactAsync(Guid agentId, string? email, string? phone, CancellationToken cancellationToken = default);

    public ValueTask SetPasswordAsync(Guid agentId, byte[] password, byte[] salt, CancellationToken cancellationToken = default);
}