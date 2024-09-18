using Lunavex.Result;
using MediatR;

namespace ProductManager.Application.Features.Auth;

/// <summary>
/// Kullanýcý giriþi için istek nesnesi.
/// </summary>
public sealed record LoginCommand(
    string EmailOrUserName,
    string Password) : IRequest<Result<LoginResponse>>;
