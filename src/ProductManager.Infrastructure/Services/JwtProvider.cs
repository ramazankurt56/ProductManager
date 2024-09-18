using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProductManager.Application.Abstractions;
using ProductManager.Application.Features.Auth;
using ProductManager.Domain.Entities;
using ProductManager.Infrastructure.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProductManager.Infrastructure.Services;

/// <summary>
/// JWT tokenlar� olu�turmak i�in hizmet sa�layan JwtProvider s�n�f�.
/// </summary>
public class JwtProvider(
    UserManager<User> userManager,
    IOptions<JwtOptions> jwtOptions) : IJwtProvider
{
    /// <summary>
    /// Belirtilen kullan�c� i�in JWT ve yenileme token� olu�turur.
    /// </summary>
    /// <param name="user">Token olu�turulacak kullan�c�.</param>
    /// <returns>LoginResponse nesnesi d�nd�r�r.</returns>
    public async Task<LoginResponse> CreateToken(User user)
    {
        // Kullan�c� bilgilerini i�eren claim'leri olu�tur.
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, user.Email ?? ""),
            new Claim(ClaimTypes.Name, user.UserName ?? "")
        };

        // Kullan�c�n�n rollerini al ve claim'lere ekle.
        var roles = await userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // Token'�n ge�erlilik s�resini belirle (1 saat).
        DateTime expires = DateTime.UtcNow.AddHours(1);

        // G�venlik anahtar�n� olu�tur.
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Value.SecretKey));

        // JWT token�n� yap�land�r ve olu�tur.
        JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
            issuer: jwtOptions.Value.Issuer,
            audience: jwtOptions.Value.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expires,
            signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512));

        // Token� yazmak i�in handler olu�tur.
        JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

        // Token� string format�nda yaz.
        string token = handler.WriteToken(jwtSecurityToken);

        // Yenileme token� olu�tur ve ge�erlilik s�resini ayarla.
        string refreshToken = Guid.NewGuid().ToString();
        DateTime refreshTokenExpires = expires.AddHours(1);

        // Kullan�c�n�n yenileme token� bilgilerini g�ncelle.
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpires = refreshTokenExpires;

        // Kullan�c� bilgisini veritaban�nda g�ncelle.
        await userManager.UpdateAsync(user);

        // Olu�turulan tokenlar� i�eren yan�t� d�nd�r.
        return new LoginResponse(token, refreshToken, refreshTokenExpires);
    }
}
