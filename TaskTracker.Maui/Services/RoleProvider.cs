using TaskTracker.Application.Common;

namespace TaskTracker.Maui.Services;

public static class RoleProvider
{
    public static IReadOnlyList<string> All { get; } =
    [
        Roles.Admin,
        Roles.ChiefProjectManager,
        Roles.ProjectManager,
        Roles.Worker
    ];
}