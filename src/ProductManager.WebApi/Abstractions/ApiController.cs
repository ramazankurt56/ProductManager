// Bu dosya, tüm controller'lar için temel sınıf olan ApiController'ı tanımlar.

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProductManager.WebApi.Abstractions;

/// <summary>
/// Tüm API controller'ları için temel sınıf.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = "Bearer")]
public abstract class ApiController : ControllerBase
{
    /// <summary>
    /// MediatR arayüzü.
    /// </summary>
    protected readonly IMediator _mediator;

    /// <summary>
    /// ApiController sınıfının yapıcısı.
    /// </summary>
    /// <param name="mediator">MediatR arayüzü.</param>
    protected ApiController(IMediator mediator)
    {
        _mediator = mediator;
    }
}
