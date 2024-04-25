using Tgs.ServiceUtility;

namespace Tgs.Agent.Core.ApplicationServices;

internal class GetAgentEventQueryService(IAgentRepository repository) : IGetService<GetAgentEventQuery, EventInfo?>
{
    private readonly IAgentRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public ValueTask<EventInfo?> GetAsync(GetAgentEventQuery query, CancellationToken cancellationToken = default)
        => _repository.GetAgentEventAsync(query.EventId, cancellationToken);
}