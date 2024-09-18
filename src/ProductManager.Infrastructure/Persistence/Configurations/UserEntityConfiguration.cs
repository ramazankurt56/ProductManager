
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductManager.Domain.Entities;

namespace ProductManager.Infrastructure.Persistence.Configurations;

/// <summary>
/// User entity'si i�in yap�land�rma kurallar�n� tan�mlar.
/// Veritaban� �emas� ve s�tun k�s�tlamalar� bu yap� i�inde belirlenir.
/// </summary>
public sealed class UserEntityConfiguration : IEntityTypeConfiguration<User>
{
    /// <summary>
    /// User entity'sinin veritaban� yap�land�rmas�n� ger�ekle�tirir.
    /// </summary>
    /// <param name="builder">User entity'si i�in yap�land�r�c�.</param>
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(p => p.FirstName).HasColumnType("varchar(50)");
        builder.Property(p => p.LastName).HasColumnType("varchar(50)");
    }
}
