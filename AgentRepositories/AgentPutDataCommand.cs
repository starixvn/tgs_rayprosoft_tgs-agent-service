using Tgs.Agent.Core;

namespace Tgs.Agent.Repositories;

internal class AgentPutDataCommand(string name, string value) : PutDataCommand(name, value)
{
    public override ValueTask ExecuteAsync(IDataCommandContext context, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (context is not AgentModifyCommandContext modifyContext)
            throw new AgentModifyCommandContextTypeErrorException();

        switch (Name)
        {
            case nameof(AgentPropertyName.RealName):
                modifyContext.Agent.RealName = Value;
                break;

            case nameof(AgentPropertyName.Remarks):
                modifyContext.Agent.Remarks = Value;
                break;

            case nameof(AgentPropertyName.Phone):
                modifyContext.Agent.Phone = Value;
                break;

            case nameof(AgentPropertyName.Email):
                modifyContext.Agent.Email = Value;
                break;

            case nameof(AgentPropertyName.Qq):
                modifyContext.Agent.Qq = Value;
                break;

            case nameof(AgentPropertyName.Wechat):
                modifyContext.Agent.Wechat = Value;
                break;

            default:
                break;
        }

        return ValueTask.CompletedTask;
    }
}