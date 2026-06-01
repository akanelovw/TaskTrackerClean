using TaskTracker.Domain.Enums;

namespace TaskTracker.Domain.Entities;

public class Project
{
    public int Id { get; private set; }

    public string Title { get; private set; } = null!;
    public string CustomerCompany { get; private set; } = null!;
    public string ExecutorCompany { get; private set; } = null!;

    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }

    public ProjectPriority Priority { get; private set; }
    public ProjectStatus Status { get; private set; }

    public string? ManagerUserId { get; private set; }

    public List<Document> Documents { get; private set; } = new();
    public List<ProjectMember> Members { get; private set; } = new();

    private Project() { } 

    public Project(
        string title,
        string customerCompany,
        string executorCompany,
        DateTime startTime,
        DateTime endTime,
        ProjectPriority priority,
        string? managerUserId)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required");

        if (string.IsNullOrWhiteSpace(customerCompany))
            throw new ArgumentException("CustomerCompany is required");

        if (string.IsNullOrWhiteSpace(executorCompany))
            throw new ArgumentException("ExecutorCompany is required");

        if (startTime > endTime)
            throw new ArgumentException("StartTime cannot be after EndTime");

        Title = title;
        CustomerCompany = customerCompany;
        ExecutorCompany = executorCompany;
        StartTime = startTime;
        EndTime = endTime;
        Priority = priority;
        ManagerUserId = managerUserId;

        Status = ProjectStatus.Active;
    }

    public void Update(
        string title,
        string customerCompany,
        string executorCompany,
        DateTime startTime,
        DateTime endTime,
        ProjectPriority priority)
    {
        EnsureNotArchived();

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required");

        if (string.IsNullOrWhiteSpace(customerCompany))
            throw new ArgumentException("CustomerCompany is required");

        if (string.IsNullOrWhiteSpace(executorCompany))
            throw new ArgumentException("ExecutorCompany is required");

        if (startTime > endTime)
            throw new ArgumentException("Invalid dates");

        Title = title;
        CustomerCompany = customerCompany;
        ExecutorCompany = executorCompany;
        StartTime = startTime;
        EndTime = endTime;
        Priority = priority;
    }

    public void AddMember(string userId)
    {
        EnsureNotArchived();

        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("UserId is required");

        if (Members.Any(x => x.UserId == userId))
            return;

        Members.Add(new ProjectMember(Id, userId));
    }

    public void RemoveMember(string userId)
    {
        EnsureNotArchived();

        var member = Members.FirstOrDefault(x => x.UserId == userId);
        if (member != null)
            Members.Remove(member);
    }

    public void ChangeManager(string userId)
    {
        EnsureNotArchived();

        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("Manager is required");

        ManagerUserId = userId;
    }

    public void ChangePriority(ProjectPriority priority)
    {
        EnsureNotArchived();
        Priority = priority;
    }

    public void ChangeStatus(ProjectStatus status)
    {
        EnsureNotArchived();
        Status = status;
    }

    public void AddDocument(Document document)
    {
        EnsureNotArchived();

        if (document == null)
            throw new ArgumentNullException(nameof(document));

        Documents.Add(document);
    }

    public void RemoveDocument(Document document)
    {
        EnsureNotArchived();

        if (document == null)
            throw new ArgumentNullException(nameof(document));

        Documents.Remove(document);
    }

    public bool HasMember(string userId)
        => Members.Any(x => x.UserId == userId);

    private void EnsureNotArchived()
    {
        if (Status == ProjectStatus.Archived)
            throw new InvalidOperationException("Archived project cannot be modified");
    }
}