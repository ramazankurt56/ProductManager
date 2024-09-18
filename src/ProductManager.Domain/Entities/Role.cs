using Microsoft.AspNetCore.Identity;

namespace ProductManager.Domain.Entities;

/// <summary>
/// <see cref="IdentityRole{Guid}"/> s�n�f�ndan t�retilmi� uygulaman�n rol s�n�f�.
/// </summary>
public sealed class Role : IdentityRole<Guid>
{
    // Ek �zellikler ve y�ntemler burada tan�mlanabilir.
}
