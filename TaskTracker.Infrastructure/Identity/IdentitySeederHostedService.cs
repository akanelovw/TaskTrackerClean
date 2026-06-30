using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TaskTracker.Application.Common;

namespace TaskTracker.Infrastructure.Identity;

public class IdentitySeederHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly AdminUserOptions _adminOptions;
    private readonly ILogger<IdentitySeederHostedService> _logger;

    public IdentitySeederHostedService(
        IServiceProvider serviceProvider,
        IOptions<AdminUserOptions> adminOptions,
        ILogger<IdentitySeederHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _adminOptions = adminOptions.Value;
        _logger = logger;
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
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
                _logger.LogInformation("Создана роль {Role}", role);
            }
        }
    }

    private async Task SeedAdmin(UserManager<AppUser> userManager)
    {
        var email = _adminOptions.Email;

        var existing = await userManager.FindByEmailAsync(email);
        if (existing != null)
        {
            _logger.LogInformation("Админ {Email} уже существует, пропускаем создание", email);
            return;
        }

        var user = new AppUser
        {
            Email = email,
            UserName = email,
            FirstName = _adminOptions.FirstName,
            LastName = _adminOptions.LastName,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, _adminOptions.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(x => x.Description));
            _logger.LogError("Не удалось создать админа: {Errors}", errors);
            throw new InvalidOperationException($"Seed admin failed: {errors}");
        }

        await userManager.AddToRoleAsync(user, Roles.Admin);
        _logger.LogInformation("Создан админ {Email}", email);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}