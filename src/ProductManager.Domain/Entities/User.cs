using Microsoft.AspNetCore.Identity;

namespace ProductManager.Domain.Entities;

/// <summary>
/// <see cref="IdentityUser{Guid}"/> s�n�f�ndan t�retilmi� uygulaman�n kullan�c� s�n�f�.
/// </summary>
public sealed class User : IdentityUser<Guid>
{
    /// <summary>
    /// Kullan�c�n�n ad�.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Kullan�c�n�n soyad�.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Kullan�c�n�n yenileme token�.
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Yenileme token�n�n ge�erlilik s�resi.
    /// </summary>
    public DateTime? RefreshTokenExpires { get; set; }
}
