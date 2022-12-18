using CourseLibrary.API.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseLibrary.API.Brokers.Storages.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    void IEntityTypeConfiguration<User>.Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.FirstName)
             .HasMaxLength(50)
             .IsRequired();

        builder.Property(u => u.LastName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.DateOfBirth)
            .IsRequired();

        builder.Property(u => u.ConcurrencyStamp)
            .HasMaxLength(255)
            .IsRequired();
    }
}
