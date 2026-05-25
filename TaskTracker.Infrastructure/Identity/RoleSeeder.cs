using Microsoft.AspNetCore.Identity;

using TaskTracker.Application.Common;

namespace TaskTracker.Infrastructure.Identity;

public static class RoleSeeder
{
    public static async Task SeedAsync(
        RoleManager<IdentityRole> roleManager)
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
            var exists =
                await roleManager.RoleExistsAsync(role);

            if (!exists)
            {
                await roleManager.CreateAsync(
                    new IdentityRole(role));
            }
        }
    }
}