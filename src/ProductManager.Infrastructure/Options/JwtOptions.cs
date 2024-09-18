/// <summary>
/// JWT (JSON Web Token) yapýlandýrma seçeneklerini tanýmlar.
/// Bu seçenekler, token'ýn oluþturulmasý ve doðrulanmasý sýrasýnda kullanýlýr.
/// </summary>
public sealed class JwtOptions
{
    /// <summary>
    /// JWT token'ý üreten tarafý belirtir (Issuer).
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// JWT token'ý kullanacak olan tarafý belirtir (Audience).
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Token'ý imzalamak ve doðrulamak için kullanýlan gizli anahtarý belirtir (SecretKey).
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;
}
