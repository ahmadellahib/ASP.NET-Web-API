using CourseLibrary.API.Models.Courses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseLibrary.API.Brokers.Storages.Configurations;

internal class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    void IEntityTypeConfiguration<Course>.Configure(EntityTypeBuilder<Course> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasMaxLength(1500);

        builder.Property(c => c.ConcurrencyStamp)
            .HasMaxLength(255)
            .IsRequired();

        builder.HasOne(c => c.Author)
           .WithMany(a => a.Courses)
           .HasForeignKey(c => c.AuthorId)
           .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.CreatedBy)
           .WithMany(c => c.CreatedCourses)
           .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.UpdatedBy)
            .WithMany(c => c.UpdatedCourses)
            .OnDelete(DeleteBehavior.NoAction);
    }
}