using Microsoft.AspNetCore.Identity;

namespace ProductManager.Domain.Entities;

/// <summary>
/// <see cref="IdentityRole{Guid}"/> sýnýfýndan türetilmiþ uygulamanýn rol sýnýfý.
/// </summary>
public sealed class Role : IdentityRole<Guid>
{
    // Ek özellikler ve yöntemler burada tanýmlanabilir.
}
