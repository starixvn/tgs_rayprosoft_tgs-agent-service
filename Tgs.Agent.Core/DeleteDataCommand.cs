namespace Tgs.Agent.Core;

public abstract class DeleteDataCommand(string name) : IDataCommand
{
    public string Name { get; } = name;

    public abstract ValueTask ExecuteAsync(IDataCommandContext context, CancellationToken cancellationToken = default);
}