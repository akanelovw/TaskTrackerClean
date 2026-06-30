namespace TaskTracker.Infrastructure.Identity;

public class AdminUserOptions
{
    public const string SectionName = "Admin";

    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string FirstName { get; set; } = "System";
    public string LastName { get; set; } = "Administrator";
}