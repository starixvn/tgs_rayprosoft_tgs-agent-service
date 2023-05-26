using NSubstitute;
using Tgs.Agent.Core;
using Tgs.Agent.Core.Application;
using Tgs.Agent.Core.Models;
using Xunit;

namespace Tgs.Agent.UnitTests;

public class AgentTests
{
    [Fact]
	public async Task Get_Child_Agent_Infos()
	{
		var fakeAgentRepository = Substitute.For<IAgentRepository>();

		var sut = new GetChildAgentInfosQuery(fakeAgentRepository);

        var parentAgentId = Guid.Parse("7c6f6c96-c0f6-ec11-8db1-70b5e82b6b7c");

        fakeAgentRepository.GetChildAgentInfosAsync(Arg.Any<Guid>()).Returns(ChildAgentInfoModelData);
        var query = new GetChildAgentInfosRequest(Guid.NewGuid());

		var response = sut.QueryAsync(query, default);

        var model = await response.FirstAsync();

        Assert.NotNull(response);
        Assert.Equal(model.ParentAgentId, parentAgentId);
    }

    private async IAsyncEnumerable<ChildAgentInfoModel> ChildAgentInfoModelData(NSubstitute.Core.CallInfo info)
    {
        yield return new Core.Models.ChildAgentInfoModel
            { AgentId = Guid.NewGuid(), AgentUsername = "username", ParentAgentId = Guid.Parse("7c6f6c96-c0f6-ec11-8db1-70b5e82b6b7c") };

        await Task.CompletedTask;
    }
}
