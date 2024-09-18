/// <summary>
/// JWT (JSON Web Token) yap�land�rma se�eneklerini tan�mlar.
/// Bu se�enekler, token'�n olu�turulmas� ve do�rulanmas� s�ras�nda kullan�l�r.
/// </summary>
public sealed class JwtOptions
{
    /// <summary>
    /// JWT token'� �reten taraf� belirtir (Issuer).
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// JWT token'� kullanacak olan taraf� belirtir (Audience).
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Token'� imzalamak ve do�rulamak i�in kullan�lan gizli anahtar� belirtir (SecretKey).
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;
}
