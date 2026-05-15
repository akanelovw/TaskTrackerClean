using Microsoft.AspNetCore.Identity;

namespace TaskTracker.Infrastructure.Identity;

public class AppUser : IdentityUser
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string? MiddleName { get; set; }

    public string FullName =>
        $"{LastName} {FirstName} {MiddleName}";
}