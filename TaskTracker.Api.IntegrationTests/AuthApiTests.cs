using TaskTracker.Api.IntegrationTests;
using TaskTracker.Api.IntegrationTests.Factories;
using System.Net.Http.Json;
public class AuthApiTests : BaseIntegrationTest
{
    public AuthApiTests(ApiFactory factory) : base(factory) { }

    [Fact]
    public async Task Login_Should_Return_OK()
    {
        var response = await Client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "test@test.com",
            password = "123"
        });

        var body = await response.Content.ReadAsStringAsync();

        Assert.True(response.IsSuccessStatusCode, body);
    }
}