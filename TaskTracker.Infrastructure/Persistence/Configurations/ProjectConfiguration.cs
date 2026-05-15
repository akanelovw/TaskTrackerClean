using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskTracker.Domain.Entities;

namespace TaskTracker.Infrastructure.Persistence.Configurations;

public class ProjectConfiguration
    : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.CustomerCompany)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.ExecutorCompany)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Priority)
            .IsRequired();

        builder.Property(x => x.ManagerUserId);

        builder.HasMany(x => x.Documents)
            .WithOne()
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Members)
            .WithOne()
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata
            .FindNavigation(nameof(Project.Documents))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Metadata
            .FindNavigation(nameof(Project.Members))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}