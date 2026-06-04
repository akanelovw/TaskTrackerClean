using TaskTracker.Api.IntegrationTests;
using TaskTracker.Api.IntegrationTests.Common;
using TaskTracker.Api.IntegrationTests.Factories;
using System.Net.Http.Json;

public class ProjectsApiTests : BaseIntegrationTest
{
    public ProjectsApiTests(ApiFactory factory) : base(factory) { }

    [Fact]
    public async Task Create_And_Get_Projects_Should_Work()
    {
        var create = await Client.PostAsJsonAsync("/api/projects", new
        {
            title = "Test",
            customerCompany = "A",
            executorCompany = "B",
            startTime = DateTime.UtcNow,
            endTime = DateTime.UtcNow.AddDays(1),
            priority = 1
        });

        var body = await create.Content.ReadAsStringAsync();
        var projectId = JsonHelper.ExtractId(body);

        var response = await Client.GetAsync("/api/projects");
        var list = await response.Content.ReadAsStringAsync();

        Assert.True(response.IsSuccessStatusCode, list);
        Assert.Contains(projectId.ToString(), list);
    }
}