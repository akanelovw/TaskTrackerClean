public class GetProjectDetailsResponse
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string CustomerCompany { get; set; } = null!;

    public string ExecutorCompany { get; set; } = null!;

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public string Status { get; set; } = null!;

    public string Priority { get; set; } = null!;

    public string? ManagerUserId { get; set; }
}