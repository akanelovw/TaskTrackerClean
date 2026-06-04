using System.Net.Http.Json;
using TaskTracker.Api.IntegrationTests;
using TaskTracker.Api.IntegrationTests.Common;
using TaskTracker.Api.IntegrationTests.Factories;

public class DocumentsApiTests : BaseIntegrationTest
{
    public DocumentsApiTests(ApiFactory factory) : base(factory) { }

    [Fact]
    public async Task Get_Project_Documents_Should_Return_OK()
    {
        var createProjectResponse = await Client.PostAsJsonAsync("/api/projects", new
        {
            title = "Test Project",
            customerCompany = "A",
            executorCompany = "B",
            startTime = DateTime.UtcNow,
            endTime = DateTime.UtcNow.AddDays(1),
            priority = 1
        });

        var projectBody = await createProjectResponse.Content.ReadAsStringAsync();

        Assert.True(
            createProjectResponse.IsSuccessStatusCode,
            projectBody);

        var projectId = JsonHelper.ExtractId(projectBody);

        var response = await Client.GetAsync(
            $"/api/documents/project/{projectId}");

        var body = await response.Content.ReadAsStringAsync();

        Assert.True(
            response.IsSuccessStatusCode,
            body);

        Assert.Contains("data", body);
    }
}