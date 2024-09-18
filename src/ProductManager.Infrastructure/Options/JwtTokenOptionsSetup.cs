
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ProductManager.Infrastructure.Options;

/// <summary>
/// JWT doðrulama parametrelerini yapýlandýrmak için kullanýlan sýnýf.
/// Bu sýnýf, JWT token doðrulama iþlemleri için gerekli ayarlarý yapar.
/// </summary>
public sealed class JwtTokenOptionsSetup(
    IOptions<JwtOptions> jwtOptions) : IPostConfigureOptions<JwtBearerOptions>
{
    /// <summary>
    /// JWT token doðrulama ayarlarýný yapýlandýrýr.
    /// </summary>
    /// <param name="name">Seçenek adý, bu örnekte kullanýlmaz.</param>
    /// <param name="options">JWT Bearer doðrulama seçenekleri.</param>
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
