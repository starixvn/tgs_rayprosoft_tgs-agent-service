using Tgs.ServiceUtility;

namespace Tgs.Agent.Core.ApplicationServices;

internal class ClearNegativeCommissionCommandService(IAgentRepository repository)
    : ICommandService<ClearNegativeCommissionCommand>
{
    private readonly IAgentRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public async ValueTask ExecuteAsync(ClearNegativeCommissionCommand command, CancellationToken cancellationToken = default)
        => await _repository.ClearNegativeCommissionAsync(command.AgentId, command.ClearByUserId, cancellationToken);
}