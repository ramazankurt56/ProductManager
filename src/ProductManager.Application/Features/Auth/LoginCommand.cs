using Lunavex.Result;
using MediatR;

namespace ProductManager.Application.Features.Auth;

/// <summary>
/// Kullan�c� giri�i i�in istek nesnesi.
/// </summary>
public sealed record LoginCommand(
    string EmailOrUserName,
    string Password) : IRequest<Result<LoginResponse>>;
