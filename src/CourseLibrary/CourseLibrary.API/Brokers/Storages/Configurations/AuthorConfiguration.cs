using CourseLibrary.API.Models.Authors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseLibrary.API.Brokers.Storages.Configurations;

public class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
    void IEntityTypeConfiguration<Author>.Configure(EntityTypeBuilder<Author> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(x => x.FirstName)
             .HasMaxLength(50)
             .IsRequired();

        builder.Property(x => x.LastName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.DateOfBirth)
            .IsRequired();

        builder.Property(x => x.MainCategory)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(t => t.ConcurrencyStamp)
            .HasMaxLength(255)
            .IsRequired();
    }
}