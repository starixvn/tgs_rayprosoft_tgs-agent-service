using Tgs.Core.Infrastructures.Security;
using Tgs.ServiceUtility;

namespace Tgs.Agent.Core.ApplicationServices;

internal class SetAgentPasswordCommandService : ICommandService<SetAgentPasswordCommand>
{
    private readonly IAgentRepository _repository;
    private readonly IPasswordHandler _passwordHandler;

    public SetAgentPasswordCommandService(
        IAgentRepository repository,
        IPasswordHandler passwordHandler)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _passwordHandler = passwordHandler ?? throw new ArgumentNullException(nameof(passwordHandler));
    }

    public async ValueTask ExecuteAsync(SetAgentPasswordCommand command, CancellationToken cancellationToken = default)
    {
        var (salt, hashedPassword) = _passwordHandler.GenerateCredential(command.Password);

        await _repository.SetPasswordAsync(
            command.AgentId,
            hashedPassword,
            salt,
            cancellationToken).ConfigureAwait(false);
    }
}