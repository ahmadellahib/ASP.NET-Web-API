using CourseLibrary.API.Models.Authors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseLibrary.API.Brokers.Storages.Configurations;

internal class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
    void IEntityTypeConfiguration<Author>.Configure(EntityTypeBuilder<Author> builder)
    {
        builder.HasKey(a => a.Id);

        builder.HasIndex(a => a.UserId)
            .IsUnique();

        builder.Property(a => a.MainCategoryId)
            .IsRequired();

        builder.Property(a => a.ConcurrencyStamp)
            .HasMaxLength(255)
            .IsRequired();

        builder.HasOne(a => a.User);
    }
}