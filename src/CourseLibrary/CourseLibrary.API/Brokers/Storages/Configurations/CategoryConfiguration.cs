using CourseLibrary.API.Models.Categories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseLibrary.API.Brokers.Storages.Configurations;

internal class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    void IEntityTypeConfiguration<Category>.Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(c => c.Id);

        builder.HasIndex(a => a.Name)
            .IsUnique();

        builder.Property(c => c.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.ConcurrencyStamp)
           .HasMaxLength(255)
           .IsRequired();

        builder.HasOne(x => x.CreatedBy)
           .WithMany(c => c.CreatedCategories)
           .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.UpdatedBy)
            .WithMany(c => c.UpdatedCategories)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
