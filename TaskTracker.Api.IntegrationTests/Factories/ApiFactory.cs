using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TaskTracker.Api.IntegrationTests.Auth;
using TaskTracker.Infrastructure.Identity;
using TaskTracker.Infrastructure.Persistence;

namespace TaskTracker.Api.IntegrationTests.Factories;

public class ApiFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = Guid.NewGuid().ToString();
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            var descriptors = services
                .Where(d =>
                    d.ServiceType.FullName != null &&
                    d.ServiceType.FullName.Contains("DbContext"))
                .ToList();

            foreach (var descriptor in descriptors)
                services.Remove(descriptor);

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase(_dbName));

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 3;
            });

            services.AddAuthentication("Test")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    "Test",
                    _ => { });

            services.PostConfigure<AuthenticationOptions>(options =>
            {
                options.DefaultAuthenticateScheme = "Test";
                options.DefaultChallengeScheme = "Test";
            });

            services.AddAuthorization();
        });
    }

    protected override void ConfigureClient(HttpClient client)
    {
        client.DefaultRequestHeaders.Add("Authorization", "Test");
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        using var scope = host.Services.CreateScope();

        var userManager =
            scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

        var existingUser =
            userManager.FindByEmailAsync("test@test.com")
                .GetAwaiter()
                .GetResult();

        if (existingUser == null)
        {
            var user = new AppUser
            {
                UserName = "test@test.com",
                Email = "test@test.com",
                FirstName = "Test",
                LastName = "User"
            };

            var result =
                userManager.CreateAsync(user, "123")
                    .GetAwaiter()
                    .GetResult();

            if (!result.Succeeded)
            {
                throw new Exception(
                    string.Join(
                        " | ",
                        result.Errors.Select(x => x.Description)));
            }
        }

        return host;
    }

}