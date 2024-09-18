namespace ProductManager.Application.Features.Auth;

/// <summary>
/// Kullan�c� giri�i sonras�nda d�nd�r�len yan�t nesnesi.
/// </summary>
public sealed record LoginResponse(
    string Token,
    string RefreshToken,
    DateTime RefreshTokenExpires
    );
