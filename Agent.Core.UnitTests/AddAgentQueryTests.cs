using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.Agent.Core;
using Tgs.Agent.Core.ApplicationServices;
using Tgs.Agent.Core.Models;
using Tgs.BrandCertificate.Clients;
using Tgs.CashService;
using Tgs.CashService.Client;
using Tgs.Core.Infrastructures.Security;
using Tgs.SerialNumberGenerator;

namespace Agent.Core.UnitTests;

public class AddAgentQueryTests
{
    [Test]
    public async Task Add_Agent()
    {
        var fakeAgentRepository = Substitute.For<IAgentRepository>();
        var fakePasswordHandler = Substitute.For<IPasswordHandler>();
        var fakeCashServiceClient = Substitute.For<ICashServiceClient>();
        var fakeBrandServiceClient = Substitute.For<IBrandServiceClient>();
        var fakeSerialNumberGenerator = Substitute.For<ISerialNumberGenerator>();
        
        var sut = new AddAgentQueryService(fakeAgentRepository,
            fakePasswordHandler,
            fakeCashServiceClient,
            fakeBrandServiceClient,
            fakeSerialNumberGenerator,
            NullLogger<AddAgentQueryService>.Instance);

        var password = "password";
        var brandId = Guid.NewGuid();
        var salt = Encoding.UTF8.GetBytes("salt");
        var pwd = Encoding.UTF8.GetBytes("pwd");
        var currencyCode = "CNY";

        _ = fakePasswordHandler
            .GenerateCredential(password)
            .Returns((salt, pwd));

        var query = new AddAgentQuery()
        {
            Password = password,
            BrandId = brandId,
            RealName = "realName",
            Email = "email",
            Remarks = "remarks",
            Phone = "phone",
            CurrencyCode = currencyCode,
        };

        _ = await sut.GetAsync(query);

        _ = fakePasswordHandler
            .Received(1)
            .GenerateCredential(Arg.Is(password));

        _ = fakeCashServiceClient
            .Received(1)
            .CreateNewCashAsync(Arg.Is(brandId), Arg.Any<string>(), Arg.Is(currencyCode), Arg.Is(CashType.Agent));

        _ = fakeAgentRepository
            .Received(1)
            .CreateAgentAsync(Arg.Any<AgentModel>());
    }
}