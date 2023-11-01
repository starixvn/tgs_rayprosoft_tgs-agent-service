namespace Tgs.Agent.Core;

public interface IDataCommand
{
    ValueTask ExecuteAsync(IDataCommandContext context, CancellationToken cancellationToken = default);
}