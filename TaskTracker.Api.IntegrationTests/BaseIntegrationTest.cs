using TaskTracker.Api.IntegrationTests.Factories;
using TaskTracker.Api.IntegrationTests.Common;

namespace TaskTracker.Api.IntegrationTests;

public abstract class BaseIntegrationTest : IClassFixture<ApiFactory>
{
    protected readonly HttpClient Client;

    protected BaseIntegrationTest(ApiFactory factory)
    {
        Client = factory.CreateClient();
    }
}