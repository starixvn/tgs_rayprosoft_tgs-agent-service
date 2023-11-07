using Tgs.Agent.Core;

namespace Tgs.Agent.Repositories;

internal class AgentDeleteDataCommand : DeleteDataCommand
{
    public AgentDeleteDataCommand(string name) : base(name)
    {
    }

    public override ValueTask ExecuteAsync(IDataCommandContext context, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (context is not AgentModifyCommandContext modifyContext)
            throw new AgentModifyCommandContextTypeErrorException();

        switch (Name)
        {
            case nameof(AgentPropertyName.RealName):
                modifyContext.Agent.RealName = null;
                break;

            case nameof(AgentPropertyName.Remarks):
                modifyContext.Agent.Remarks = null;
                break;

            case nameof(AgentPropertyName.Phone):
                modifyContext.Agent.Phone = null;
                break;

            case nameof(AgentPropertyName.Email):
                modifyContext.Agent.Email = null;
                break;

            case nameof(AgentPropertyName.Qq):
                modifyContext.Agent.Qq = null;
                break;

            case nameof(AgentPropertyName.Wechat):
                modifyContext.Agent.Wechat = null;
                break;

            default:
                break;
        }

        return ValueTask.CompletedTask;
    }
}