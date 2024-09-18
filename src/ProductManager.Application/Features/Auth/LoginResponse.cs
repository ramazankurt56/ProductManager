namespace ProductManager.Application.Features.Auth;

/// <summary>
/// Kullanýcý giriþi sonrasýnda döndürülen yanýt nesnesi.
/// </summary>
public sealed record LoginResponse(
    string Token,
    string RefreshToken,
    DateTime RefreshTokenExpires
    );
