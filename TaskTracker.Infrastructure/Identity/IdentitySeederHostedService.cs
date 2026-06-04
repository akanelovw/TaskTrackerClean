using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TaskTracker.Application.Common;

namespace TaskTracker.Infrastructure.Identity;

public class IdentitySeederHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public IdentitySeederHostedService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        await SeedRoles(roleManager);

        await SeedAdmin(userManager);
    }

    private async Task SeedRoles(RoleManager<IdentityRole> roleManager)
    {
        var roles = new[]
        {
            Roles.Admin,
            Roles.ChiefProjectManager,
            Roles.ProjectManager,
            Roles.Worker
        };

        foreach (var role in roles)
        {
            var exists = await roleManager.RoleExistsAsync(role);
            if (!exists)
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    private async Task SeedAdmin(UserManager<AppUser> userManager)
    {
        var email = "admin@tasktracker.com";

        var user = await userManager.FindByEmailAsync(email);
        if (user != null)
            return;

        user = new AppUser
        {
            Email = email,
            UserName = email,
            FirstName = "System",
            LastName = "Admin",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, "Admin123!");

        if (!result.Succeeded)
        {
            throw new Exception(string.Join(", ", result.Errors.Select(x => x.Description)));
        }

        await userManager.AddToRoleAsync(user, Roles.Admin);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}