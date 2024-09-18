using Microsoft.AspNetCore.Identity;

namespace ProductManager.Domain.Entities;

/// <summary>
/// <see cref="IdentityUser{Guid}"/> sýnýfýndan türetilmiþ uygulamanýn kullanýcý sýnýfý.
/// </summary>
public sealed class User : IdentityUser<Guid>
{
    /// <summary>
    /// Kullanýcýnýn adý.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Kullanýcýnýn soyadý.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Kullanýcýnýn yenileme tokený.
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Yenileme tokenýnýn geçerlilik süresi.
    /// </summary>
    public DateTime? RefreshTokenExpires { get; set; }
}
