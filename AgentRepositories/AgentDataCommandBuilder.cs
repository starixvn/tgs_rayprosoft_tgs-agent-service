using Tgs.Agent.Core;

namespace Tgs.Agent.Repositories;

internal class AgentDataCommandBuilder : IDataCommandBuilder
{
    public IDataCommand CreateDeleteCommand(string name)
        => new AgentDeleteDataCommand(name);

    public IDataCommand CreatePutCommand(string name, string value)
        => new AgentPutDataCommand(name, value);
}