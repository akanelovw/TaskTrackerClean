namespace TaskTracker.Application.Users.GetUserById;

public class GetUserByIdResponse
{
    public string Id { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string MiddleName { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Role { get; set; } = null!;
}