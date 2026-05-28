using Microsoft.AspNetCore.Identity;

using TaskTracker.Application.Common;

namespace TaskTracker.Infrastructure.Identity;

public static class AdminSeeder
{
    public static async Task SeedAsync(
        UserManager<AppUser> userManager)
    {
        var email = "admin@tasktracker.com";

        var exists =
            await userManager.FindByEmailAsync(email);

        if (exists != null)
            return;

        var admin = new AppUser
        {
            Email = email,
            UserName = email,
            FirstName = "System",
            LastName = "Administrator"
        };

        var result =
            await userManager.CreateAsync(
                admin,
                "Admin123!");

        if (!result.Succeeded)
        {
            throw new Exception(
                string.Join(", ",
                    result.Errors.Select(x => x.Description)));
        }

        await userManager.AddToRoleAsync(
            admin,
            Roles.Admin);
    }
}