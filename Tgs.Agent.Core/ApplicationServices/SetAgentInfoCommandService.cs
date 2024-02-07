using Tgs.Agent.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.Agent.Core.ApplicationServices;

internal class SetAgentInfoCommandService(IAgentRepository repository) : ICommandService<SetAgentInfoCommand>
{
    private readonly IAgentRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public ValueTask ExecuteAsync(SetAgentInfoCommand command, CancellationToken cancellationToken = default)
        => _repository.SetAgentInfoAsync(new UpdateAgentModel(command.AgentId,
                command.ParentAgentId,
                command.AgentStatus,
                command.AgentLevelType,
                command.AgentConfigStatus,
                command.NegativeAgentDividendCarryForward,
                command.RealName,
                command.Remarks,
                command.EditorUsername),
            cancellationToken);
}