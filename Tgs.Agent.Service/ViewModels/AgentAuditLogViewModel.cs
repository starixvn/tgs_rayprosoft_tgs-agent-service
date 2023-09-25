using System.Text.Json.Serialization;
using Tgs.Agent.Core.Models.Enums;

namespace Tgs.Agent.Service.ViewModels;

public record AgentAuditLogViewModel(
    Guid AgentId,    
    AgentOperation Operation,
    string EditUsername);