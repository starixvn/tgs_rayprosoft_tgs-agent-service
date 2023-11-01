namespace Tgs.Agent.Core;

public interface IDataCommandBuilder
{
    IDataCommand CreateDeleteCommand(string name);

    IDataCommand CreatePutCommand(string name, string value);
}