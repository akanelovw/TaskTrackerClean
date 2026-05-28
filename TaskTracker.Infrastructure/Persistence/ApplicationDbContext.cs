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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Project>(cfg =>
        {
            cfg.HasKey(x => x.Id);

            cfg.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            cfg.Property(x => x.CustomerCompany)
                .IsRequired();

            cfg.Property(x => x.ExecutorCompany)
                .IsRequired();

            cfg.HasMany(typeof(Document), "_documents")
                .WithOne()
                .HasForeignKey("ProjectId")
                .OnDelete(DeleteBehavior.Cascade);

            cfg.HasMany(typeof(ProjectMember), "_members")
                .WithOne()
                .HasForeignKey("ProjectId")
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}