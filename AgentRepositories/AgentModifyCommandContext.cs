using Tgs.Agent.Core;
using Tgs.Agent.Repositories.Entities;

namespace Tgs.Agent.Repositories;
public record struct AgentModifyCommandContext(
    Entities.Agent Agent,
    TgsContext TgsContext) : IDataCommandContext;