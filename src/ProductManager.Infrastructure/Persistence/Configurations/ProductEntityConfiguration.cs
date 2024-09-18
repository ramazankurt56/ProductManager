using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductManager.Domain.Entities;

namespace ProductManager.Infrastructure.Persistence.Configurations;

/// <summary>
/// Product entity'si i�in yap�land�rma kurallar�n� tan�mlar.
/// Veritaban� �emas� ve s�tun k�s�tlamalar� bu yap� i�inde belirlenir.
/// </summary>
public class ProductEntityConfiguration : IEntityTypeConfiguration<Product>
{
    /// <summary>
    /// Product entity'sinin veritaban� yap�land�rmas�n� ger�ekle�tirir.
    /// </summary>
    /// <param name="builder">Product entity'si i�in yap�land�r�c�.</param>
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(s => s.Id);

        builder.ToTable("Products");

        builder.Property(s => s.Name)
            .HasMaxLength(100)
            .IsRequired();

        // Name �zelli�i i�in benzersiz bir indeks olu�turuyoruz
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
