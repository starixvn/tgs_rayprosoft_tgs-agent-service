namespace Tgs.Agent.Core;

public abstract class DeleteDataCommand : IDataCommand
{
    public string Name { get; }

    public DeleteDataCommand(string name)
    {
        Name = name;
    }

    public abstract ValueTask ExecuteAsync(IDataCommandContext context, CancellationToken cancellationToken = default);
}