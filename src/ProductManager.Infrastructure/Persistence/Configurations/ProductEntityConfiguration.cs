using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductManager.Domain.Entities;

namespace ProductManager.Infrastructure.Persistence.Configurations;

/// <summary>
/// Product entity'si için yapýlandýrma kurallarýný tanýmlar.
/// Veritabaný þemasý ve sütun kýsýtlamalarý bu yapý içinde belirlenir.
/// </summary>
public class ProductEntityConfiguration : IEntityTypeConfiguration<Product>
{
    /// <summary>
    /// Product entity'sinin veritabaný yapýlandýrmasýný gerçekleþtirir.
    /// </summary>
    /// <param name="builder">Product entity'si için yapýlandýrýcý.</param>
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(s => s.Id);

        builder.ToTable("Products");

        builder.Property(s => s.Name)
            .HasMaxLength(100)
            .IsRequired();

        // Name özelliði için benzersiz bir indeks oluþturuyoruz
        builder.HasIndex(s => s.Name)
            .IsUnique();

        builder.Property(p => p.Description)
            .HasMaxLength(500);

        builder
            .Property(p => p.Price)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
    }
}
