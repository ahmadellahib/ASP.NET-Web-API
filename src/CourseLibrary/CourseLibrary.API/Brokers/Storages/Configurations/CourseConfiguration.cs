using CourseLibrary.API.Models.Courses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseLibrary.API.Brokers.Storages.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    void IEntityTypeConfiguration<Course>.Configure(EntityTypeBuilder<Course> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Title)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasMaxLength(1500);

        builder.HasOne(c => c.Author)
            .WithMany(a => a.Courses)
            .HasForeignKey(c => c.AuthorId)
            .OnDelete(DeleteBehavior.NoAction);

        //builder.HasData(
        //    new Course()
        //    {
        //        Id = Guid.Parse("5b1c2b4d-48c7-402a-80c3-cc796ad49c6b"),
        //        Title = "Commandeering a Ship Without Getting Caught",
        //        AuthorId = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
        //        Description = "Commandeering a ship in rough waters isn't easy.  Commandeering it without getting caught is even harder.  In this course you'll learn how to sail away and avoid those pesky musketeers."
        //    },
        //   new Course()
        //   {
        //       Id = Guid.Parse("d8663e5e-7494-4f81-8739-6e0de1bea7ee"),
        //       Title = "Overthrowing Mutiny",
        //       AuthorId = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
        //       Description = "In this course, the author provides tips to avoid, or, if needed, overthrow pirate mutiny."
        //   },
        //   new Course()
        //   {
        //       Id = Guid.Parse("d173e20d-159e-4127-9ce9-b0ac2564ad97"),
        //       Title = "Avoiding Brawls While Drinking as Much Rum as You Desire",
        //       AuthorId = Guid.Parse("da2fd609-d754-4feb-8acd-c4f9ff13ba96"),
        //       Description = "Every good pirate loves rum, but it also has a tendency to get you into trouble.  In this course you'll learn how to avoid that.  This new exclusive edition includes an additional chapter on how to run fast without falling while drunk."
        //   }
        //   );
    }
}
