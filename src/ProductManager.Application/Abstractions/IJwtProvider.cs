using ProductManager.Application.Features.Auth;
using ProductManager.Domain.Entities;

namespace ProductManager.Application.Abstractions;

/// <summary>
/// Kullanıcılar için JWT tokenları oluşturmayı sağlayan interface.
/// </summary>
public interface IJwtProvider
{
    /// <summary>
    /// Belirtilen kullanıcı için bir JWT tokenı oluşturur.
    /// </summary>
    /// <param name="user">Token oluşturulacak kullanıcı nesnesi.</param>
    /// <returns>LoginResponse nesnesini asenkron olarak döndürür.</returns>
    Task<LoginResponse> CreateToken(User user);
}
