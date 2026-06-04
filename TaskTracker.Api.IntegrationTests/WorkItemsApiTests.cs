using System.Net.Http.Json;
using TaskTracker.Api.IntegrationTests;
using TaskTracker.Api.IntegrationTests.Common;
using TaskTracker.Api.IntegrationTests.Factories;

public class WorkItemsApiTests : BaseIntegrationTest
{
    public WorkItemsApiTests(ApiFactory factory) : base(factory) { }

    [Fact]
    public async Task Create_And_Get_WorkItems_Should_Work()
    {
        var project = await Client.PostAsJsonAsync("/api/projects", new
        {
            title = "Test",
            customerCompany = "A",
            executorCompany = "B",
            startTime = DateTime.UtcNow,
            endTime = DateTime.UtcNow.AddDays(1),
            priority = 1
        });

        var projectBody = await project.Content.ReadAsStringAsync();
        var projectId = JsonHelper.ExtractId(projectBody);

        var work = await Client.PostAsJsonAsync("/api/workitems", new
        {
            title = "Task",
            projectId = projectId,
            priority = 1
        });

        var workBody = await work.Content.ReadAsStringAsync();
        var workId = JsonHelper.ExtractId(workBody);

        var response = await Client.GetAsync("/api/workitems");
        var list = await response.Content.ReadAsStringAsync();

        Assert.True(response.IsSuccessStatusCode, list);
        Assert.Contains(workId.ToString(), list);
    }
}