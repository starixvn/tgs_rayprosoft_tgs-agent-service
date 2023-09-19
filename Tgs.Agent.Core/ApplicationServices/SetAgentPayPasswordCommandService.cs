using Tgs.Core.Infrastructures.Security;
using Tgs.ServiceUtility;

namespace Tgs.Agent.Core.ApplicationServices;

internal class SetAgentPayPasswordCommandService : ICommandService<SetAgentPayPasswordCommand>
{
    private readonly IAgentRepository _repository;
    private readonly IPasswordHandler _passwordHandler;

    public SetAgentPayPasswordCommandService(
        IAgentRepository agentRepository,
        IPasswordHandler passwordHandler)
    {
        _repository = agentRepository ?? throw new ArgumentNullException(nameof(agentRepository));
        _passwordHandler = passwordHandler ?? throw new ArgumentNullException(nameof(passwordHandler));
    }

    public async ValueTask ExecuteAsync(SetAgentPayPasswordCommand command, CancellationToken cancellationToken = default)
    {
        var (paySalt, hashedPayPassword) = _passwordHandler.GenerateCredential(command.PayPassword);

        await _repository.SetPayPasswordAsync(
            command.AgentId,
            hashedPayPassword,
            paySalt,
            cancellationToken).ConfigureAwait(false);
    }
}