// Bu dosya, kimlik doğrulama işlemlerini yöneten AuthController sınıfını tanımlar.

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductManager.Application.Features.Auth;
using ProductManager.WebApi.Abstractions;

namespace ProductManager.WebApi.Controllers;

/// <summary>
/// Kimlik doğrulama işlemlerini yöneten controller.
/// </summary>
[AllowAnonymous]
public sealed class AuthController : ApiController
{
    /// <summary>
    /// AuthController sınıfının yapıcısı.
    /// </summary>
    /// <param name="mediator">MediatR arayüzü.</param>
    public AuthController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// Kullanıcı girişi yapar ve JWT token üretir.
    /// </summary>
    /// <param name="request">Giriş komutu.</param>
    /// <param name="cancellationToken">İptal belirteci.</param>
    /// <returns>Giriş işleminin sonucu ve üretilen token.</returns>
    [HttpPost]
    public async Task<IActionResult> Login(LoginCommand request, CancellationToken cancellationToken)
    {
        // Giriş komutunu gönderir ve yanıtı alır.
        var loginResponse = await _mediator.Send(request, cancellationToken);

        // Yanıtı ve durum kodunu döndürür.
        return StatusCode(loginResponse.StatusCode, loginResponse);
    }
}
