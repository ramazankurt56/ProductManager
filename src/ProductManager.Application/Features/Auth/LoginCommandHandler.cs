using Lunavex.Result;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductManager.Application.Abstractions;
using ProductManager.Domain.Entities;

namespace ProductManager.Application.Features.Auth;

/// <summary>
/// Kullan�c� giri�i iste�ini i�leyen class.
/// </summary>
internal sealed class LoginCommandHandler(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    IJwtProvider jwtProvider,
    ILogger<LoginCommandHandler> logger) : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    /// <summary>
    /// Kullan�c� giri� iste�ini i�ler.
    /// </summary>
    /// <param name="request">LoginCommand iste�i.</param>
    /// <param name="cancellationToken">�ptal token�.</param>
    /// <returns>LoginResponse i�eren Result nesnesi.</returns>
    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Kullan�c�n�n giri� denemesi loglan�r.
        logger.LogInformation($"Login attempt started for {request.EmailOrUserName}");

        // Kullan�c� ad� veya e-posta adresine g�re kullan�c� aran�r.
        User? user = await userManager.Users
            .FirstOrDefaultAsync(p =>
            p.UserName == request.EmailOrUserName ||
            p.Email == request.EmailOrUserName,
            cancellationToken);

        if (user is null)
        {
            // Kullan�c� bulunamad���nda loglama yap�l�r.
            logger.LogWarning($"User not found: {request.EmailOrUserName}");
            return (404, "User not found.");
        }

        // Kullan�c� bulundu, �ifre kontrol� yap�lacak.
        logger.LogInformation($"User found: {request.EmailOrUserName}, attempting password check");

        // Kullan�c�n�n �ifresi kontrol edilir.
        SignInResult signInResult = await signInManager.CheckPasswordSignInAsync(user, request.Password, true);

        if (signInResult.IsLockedOut)
        {
            TimeSpan? timeSpan = user.LockoutEnd - DateTime.UtcNow;

            // E�er kilitlenme s�resi null ise, bu bir hata durumu olmal�d�r.
            if (timeSpan is null)
            {
                // Hata durumu: Kilitlenme s�resi belirtilmemi�.
                logger.LogError($"User account lockout end is not defined: {request.EmailOrUserName}");
                return (500, "An error occurred. Please contact support.");
            }

            // Kullan�c� kilitli ve kilit s�resi belirlenmi�.
            logger.LogWarning($"User account is locked: {request.EmailOrUserName}, lockout time remaining: {Math.Ceiling(timeSpan.Value.TotalMinutes)} minutes");
            return (400, $"Your account is locked for {Math.Ceiling(timeSpan.Value.TotalMinutes)} minutes due to 3 failed login attempts.");
        }


        if (signInResult.IsNotAllowed)
        {
            // Kullan�c�n�n e-posta do�rulamas� yap�lmam��.
            logger.LogWarning($"Login not allowed for user {request.EmailOrUserName}: Email not confirmed.");
            return (400, "Your email is not confirmed.");
        }

        if (!signInResult.Succeeded)
        {
            // Yanl�� �ifre girildi.
            logger.LogWarning($"Incorrect password for user {request.EmailOrUserName}");
            return (400, "Incorrect password.");
        }

        // �ifre do�ru, JWT token� olu�turuluyor.
        logger.LogInformation($"Password correct for user {request.EmailOrUserName}, generating JWT token");

        var loginResponse = await jwtProvider.CreateToken(user);

        // Ba�ar�l� giri� loglan�r.
        logger.LogInformation($"Login successful for {request.EmailOrUserName}");

        return loginResponse;
    }
}
