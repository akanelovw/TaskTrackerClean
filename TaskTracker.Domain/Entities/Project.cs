using TaskTracker.Domain.Enums;

namespace TaskTracker.Domain.Entities;

public class Project
{
    public int Id { get; private set; }

    public string Title { get; private set; }

    public string CustomerCompany { get; private set; }

    public string ExecutorCompany { get; private set; }

    public DateTime StartTime { get; private set; }

    public DateTime EndTime { get; private set; }

    public ProjectPriority Priority { get; private set; }

    public string? ManagerUserId { get; private set; }

    private readonly List<Document> _documents = new();

    public IReadOnlyCollection<Document> Documents => _documents;

    private readonly List<ProjectMember> _members = new();
    public IReadOnlyCollection<ProjectMember> Members => _members;

    private Project() { } // EF

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

        if (startTime > endTime)
            throw new ArgumentException("StartTime cannot be after EndTime");

        Title = title;
        CustomerCompany = customerCompany;
        ExecutorCompany = executorCompany;
        StartTime = startTime;
        EndTime = endTime;
        Priority = priority;
        ManagerUserId = managerUserId;
    }

    public void Update(
        string title,
        string customerCompany,
        string executorCompany,
        DateTime startTime,
        DateTime endTime,
        ProjectPriority priority)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required");

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
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("UserId is required");

        if (_members.Any(x => x.UserId == userId))
            return;

        _members.Add(new ProjectMember(Id, userId));
    }

    public void RemoveMember(string userId)
    {
        var member = _members.FirstOrDefault(x => x.UserId == userId);
        if (member != null)
            _members.Remove(member);
    }

    public void ChangeManager(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("Manager is required");

        ManagerUserId = userId;
    }

    public void ChangePriority(ProjectPriority priority)
    {
        Priority = priority;
    }
    public void AddDocument(Document document)
    {
        _documents.Add(document);
    }
    public void RemoveDocument(Document document)
    {
        _documents.Remove(document);
    }

    public bool HasMember(string userId)
        => _members.Any(x => x.UserId == userId);
}

