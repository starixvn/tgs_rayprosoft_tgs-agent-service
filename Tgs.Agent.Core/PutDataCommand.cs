namespace Tgs.Agent.Core;

public abstract class PutDataCommand(string name, string value) : IDataCommand
{
    public string Name { get; } = name;

    public string Value { get; } = value;

    public abstract ValueTask ExecuteAsync(IDataCommandContext context, CancellationToken cancellationToken = default);
}