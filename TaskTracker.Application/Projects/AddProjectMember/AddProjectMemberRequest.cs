using System;
namespace TaskTracker.Application.Projects.AddProjectMember;

public class AddProjectMemberRequest
{
    public int ProjectId { get; set; }

    public string UserId { get; set; } = null!;
}