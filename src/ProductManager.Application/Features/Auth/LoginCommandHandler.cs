using Lunavex.Result;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductManager.Application.Abstractions;
using ProductManager.Domain.Entities;

namespace ProductManager.Application.Features.Auth;

/// <summary>
/// Kullanýcý giriþi isteðini iþleyen class.
/// </summary>
internal sealed class LoginCommandHandler(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    IJwtProvider jwtProvider,
    ILogger<LoginCommandHandler> logger) : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    /// <summary>
    /// Kullanýcý giriþ isteðini iþler.
    /// </summary>
    /// <param name="request">LoginCommand isteði.</param>
    /// <param name="cancellationToken">Ýptal tokený.</param>
    /// <returns>LoginResponse içeren Result nesnesi.</returns>
    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Kullanýcýnýn giriþ denemesi loglanýr.
        logger.LogInformation($"Login attempt started for {request.EmailOrUserName}");

        // Kullanýcý adý veya e-posta adresine göre kullanýcý aranýr.
        User? user = await userManager.Users
            .FirstOrDefaultAsync(p =>
            p.UserName == request.EmailOrUserName ||
            p.Email == request.EmailOrUserName,
            cancellationToken);

        if (user is null)
        {
            // Kullanýcý bulunamadýðýnda loglama yapýlýr.
            logger.LogWarning($"User not found: {request.EmailOrUserName}");
            return (404, "User not found.");
        }

        // Kullanýcý bulundu, þifre kontrolü yapýlacak.
        logger.LogInformation($"User found: {request.EmailOrUserName}, attempting password check");

        // Kullanýcýnýn þifresi kontrol edilir.
        SignInResult signInResult = await signInManager.CheckPasswordSignInAsync(user, request.Password, true);

        if (signInResult.IsLockedOut)
        {
            TimeSpan? timeSpan = user.LockoutEnd - DateTime.UtcNow;

            // Eðer kilitlenme süresi null ise, bu bir hata durumu olmalýdýr.
            if (timeSpan is null)
            {
                // Hata durumu: Kilitlenme süresi belirtilmemiþ.
                logger.LogError($"User account lockout end is not defined: {request.EmailOrUserName}");
                return (500, "An error occurred. Please contact support.");
            }

            // Kullanýcý kilitli ve kilit süresi belirlenmiþ.
            logger.LogWarning($"User account is locked: {request.EmailOrUserName}, lockout time remaining: {Math.Ceiling(timeSpan.Value.TotalMinutes)} minutes");
            return (400, $"Your account is locked for {Math.Ceiling(timeSpan.Value.TotalMinutes)} minutes due to 3 failed login attempts.");
        }


        if (signInResult.IsNotAllowed)
        {
            // Kullanýcýnýn e-posta doðrulamasý yapýlmamýþ.
            logger.LogWarning($"Login not allowed for user {request.EmailOrUserName}: Email not confirmed.");
            return (400, "Your email is not confirmed.");
        }

        if (!signInResult.Succeeded)
        {
            // Yanlýþ þifre girildi.
            logger.LogWarning($"Incorrect password for user {request.EmailOrUserName}");
            return (400, "Incorrect password.");
        }

        // Þifre doðru, JWT tokený oluþturuluyor.
        logger.LogInformation($"Password correct for user {request.EmailOrUserName}, generating JWT token");

        var loginResponse = await jwtProvider.CreateToken(user);

        // Baþarýlý giriþ loglanýr.
        logger.LogInformation($"Login successful for {request.EmailOrUserName}");

        return loginResponse;
    }
}
