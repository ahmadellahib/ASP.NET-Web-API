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

        builder.HasData(
            new Author()
            {
                Id = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
                FirstName = "Berry",
                LastName = "Griffin Beak Eldritch",
                DateOfBirth = new DateTime(1980, 7, 23),
                MainCategory = "Ships"
            },
            new Author()
            {
                Id = Guid.Parse("da2fd609-d754-4feb-8acd-c4f9ff13ba96"),
                FirstName = "Nancy",
                LastName = "Swashbuckler Rye",
                DateOfBirth = new DateTime(1978, 5, 21),
                MainCategory = "Rum"
            }
            );
    }
}
