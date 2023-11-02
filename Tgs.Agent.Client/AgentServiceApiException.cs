namespace Tgs.Agent.Client;

public class AgentServiceApiException : Exception
{
    public AgentServiceApiException(string? message)
        : base(message)
    {
    }
}