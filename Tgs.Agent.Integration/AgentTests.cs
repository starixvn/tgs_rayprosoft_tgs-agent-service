using Microsoft.Extensions.DependencyInjection;
using Tgs.Agent.Client;
using Tgs.Agent.Core.RepositoryModels;

namespace Tgs.Agent.IntegrationTest
{
    public class AgentTests
    {
        //[Fact]
        //public async Task Test()
        //{
        //    await using var web = new AgentTestApplication();

        //    var httpClient = web.CreateClient();

        //    var sut = new AgentClient(httpClient);

        //    var dbContext = web.Services.GetRequiredService<tgsContext>();

        //    var agentId = Guid.NewGuid();
        //    var parentAgentId = Guid.NewGuid();

        //    _ = await dbContext.Agents.AddAsync(new Agents
        //    {
        //        Id = agentId,
        //        ParentAgentId = parentAgentId,
        //        Password = Array.Empty<byte>(),
        //        Salt = Array.Empty<byte>(),
        //        Username = "agent1",
        //        RealName = "agent1",
        //        DefaultWalletId = Guid.NewGuid(),
        //        WithdrawPassword = Array.Empty<byte>(),
        //        WithdrawSalt = Array.Empty<byte>(),
        //        InvitationCode = "InvitationCode",
        //        Path = $"{parentAgentId:N}.{agentId:N}"
        //    });

        //    _ = await dbContext.SaveChangesAsync();

        //    var response =  sut.GetChildAgentInfosAsync(new(parentAgentId));

        //    Assert.NotNull(response);
        //    Assert.Equal(response.FirstAsync().Result.ParentAgentId, parentAgentId);
        //}
    }
}
