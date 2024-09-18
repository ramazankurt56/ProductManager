
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductManager.Domain.Entities;

namespace ProductManager.Infrastructure.Persistence.Configurations;

/// <summary>
/// User entity'si için yapýlandýrma kurallarýný tanýmlar.
/// Veritabaný þemasý ve sütun kýsýtlamalarý bu yapý içinde belirlenir.
/// </summary>
public sealed class UserEntityConfiguration : IEntityTypeConfiguration<User>
{
    /// <summary>
    /// User entity'sinin veritabaný yapýlandýrmasýný gerçekleþtirir.
    /// </summary>
    /// <param name="builder">User entity'si için yapýlandýrýcý.</param>
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(p => p.FirstName).HasColumnType("varchar(50)");
        builder.Property(p => p.LastName).HasColumnType("varchar(50)");
    }
}
