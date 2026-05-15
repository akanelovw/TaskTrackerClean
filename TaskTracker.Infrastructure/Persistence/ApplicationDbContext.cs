using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Domain.Entities;
using TaskTracker.Infrastructure.Identity;

namespace TaskTracker.Infrastructure.Persistence;

public class ApplicationDbContext
    : IdentityDbContext<AppUser>
{
    public DbSet<Project> Projects => Set<Project>();

    public DbSet<WorkItem> WorkItems => Set<WorkItem>();

    public DbSet<Document> Documents => Set<Document>();

    public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);
    }
}