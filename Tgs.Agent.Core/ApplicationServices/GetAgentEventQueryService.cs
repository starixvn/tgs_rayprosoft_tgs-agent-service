using Tgs.ServiceUtility;

namespace Tgs.Agent.Core.ApplicationServices;

internal class GetAgentEventQueryService : IGetService<GetAgentEventQuery, EventInfo?>
{
    private readonly IAgentRepository _repository;

    public GetAgentEventQueryService(IAgentRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public ValueTask<EventInfo?> GetAsync(GetAgentEventQuery query, CancellationToken cancellationToken = default)
        => _repository.GetAgentEventAsync(query.EventId, cancellationToken);
}