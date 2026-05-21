namespace TaskTracker.Application.WorkItems.AssignUser;

public class AssignUserRequest
{
    public int WorkItemId { get; set; }
    public string UserId { get; set; }
}