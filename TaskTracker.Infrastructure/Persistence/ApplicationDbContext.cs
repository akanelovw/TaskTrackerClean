using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Domain.Entities;
using TaskTracker.Infrastructure.Identity;

namespace TaskTracker.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<AppUser>
{
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<WorkItem> WorkItems => Set<WorkItem>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ================= PROJECT =================
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

            cfg.Property(x => x.ManagerUserId);

            cfg.HasMany(x => x.Documents)
                .WithOne()
                .HasForeignKey("ProjectId")
                .OnDelete(DeleteBehavior.Cascade);

            cfg.HasMany(x => x.Members)
                .WithOne()
                .HasForeignKey("ProjectId")
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ================= PROJECT MEMBER =================
        modelBuilder.Entity<ProjectMember>(cfg =>
        {
            cfg.HasKey(x => new { x.ProjectId, x.UserId });

            cfg.Property(x => x.UserId)
                .IsRequired();

            cfg.Property(x => x.ProjectId)
                .IsRequired();
        });

        // ================= WORK ITEM =================
        modelBuilder.Entity<WorkItem>(cfg =>
        {
            cfg.HasKey(x => x.Id);

            cfg.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            cfg.Property(x => x.CreatedByUserId)
                .IsRequired();

            cfg.Property(x => x.ProjectId)
                .IsRequired();

            cfg.Property(x => x.AssignedUserId);
        });

        // ================= DOCUMENT =================
        modelBuilder.Entity<Document>(cfg =>
        {
            cfg.HasKey("Id");

            cfg.Property<int>("ProjectId");
        });
    }
}