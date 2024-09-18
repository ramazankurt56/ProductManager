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
/// JWT tokenlarý oluþturmak için hizmet saðlayan JwtProvider sýnýfý.
/// </summary>
public class JwtProvider(
    UserManager<User> userManager,
    IOptions<JwtOptions> jwtOptions) : IJwtProvider
{
    /// <summary>
    /// Belirtilen kullanýcý için JWT ve yenileme tokený oluþturur.
    /// </summary>
    /// <param name="user">Token oluþturulacak kullanýcý.</param>
    /// <returns>LoginResponse nesnesi döndürür.</returns>
    public async Task<LoginResponse> CreateToken(User user)
    {
        // Kullanýcý bilgilerini içeren claim'leri oluþtur.
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, user.Email ?? ""),
            new Claim(ClaimTypes.Name, user.UserName ?? "")
        };

        // Kullanýcýnýn rollerini al ve claim'lere ekle.
        var roles = await userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // Token'ýn geçerlilik süresini belirle (1 saat).
        DateTime expires = DateTime.UtcNow.AddHours(1);

        // Güvenlik anahtarýný oluþtur.
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Value.SecretKey));

        // JWT tokenýný yapýlandýr ve oluþtur.
        JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
            issuer: jwtOptions.Value.Issuer,
            audience: jwtOptions.Value.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expires,
            signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512));

        // Tokený yazmak için handler oluþtur.
        JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

        // Tokený string formatýnda yaz.
        string token = handler.WriteToken(jwtSecurityToken);

        // Yenileme tokený oluþtur ve geçerlilik süresini ayarla.
        string refreshToken = Guid.NewGuid().ToString();
        DateTime refreshTokenExpires = expires.AddHours(1);

        // Kullanýcýnýn yenileme tokený bilgilerini güncelle.
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpires = refreshTokenExpires;

        // Kullanýcý bilgisini veritabanýnda güncelle.
        await userManager.UpdateAsync(user);

        // Oluþturulan tokenlarý içeren yanýtý döndür.
        return new LoginResponse(token, refreshToken, refreshTokenExpires);
    }
}
