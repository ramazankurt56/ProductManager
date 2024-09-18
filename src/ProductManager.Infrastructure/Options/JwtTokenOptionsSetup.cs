
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ProductManager.Infrastructure.Options;

/// <summary>
/// JWT do�rulama parametrelerini yap�land�rmak i�in kullan�lan s�n�f.
/// Bu s�n�f, JWT token do�rulama i�lemleri i�in gerekli ayarlar� yapar.
/// </summary>
public sealed class JwtTokenOptionsSetup(
    IOptions<JwtOptions> jwtOptions) : IPostConfigureOptions<JwtBearerOptions>
{
    /// <summary>
    /// JWT token do�rulama ayarlar�n� yap�land�r�r.
    /// </summary>
    /// <param name="name">Se�enek ad�, bu �rnekte kullan�lmaz.</param>
    /// <param name="options">JWT Bearer do�rulama se�enekleri.</param>
    public void PostConfigure(string? name, JwtBearerOptions options)
    {
        options.TokenValidationParameters.ValidateIssuer = true;
        options.TokenValidationParameters.ValidateAudience = true;
        options.TokenValidationParameters.ValidateLifetime = true;
        options.TokenValidationParameters.ValidateIssuerSigningKey = true;
        options.TokenValidationParameters.ValidIssuer = jwtOptions.Value.Issuer;
        options.TokenValidationParameters.ValidAudience = jwtOptions.Value.Audience;
        options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtOptions.Value.SecretKey));
    }
}
