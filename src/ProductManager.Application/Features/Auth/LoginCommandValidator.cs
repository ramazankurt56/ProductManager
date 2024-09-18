using FluentValidation;

namespace ProductManager.Application.Features.Auth;

/// <summary>
/// LoginCommand i�in validation  kurallar�n� tan�mlar.
/// </summary>
public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    /// <summary>
    /// Yeni bir instance olu�turur ve validation kurallar�n� tan�mlar.
    /// </summary>
    public LoginCommandValidator()
    {
        // Email veya kullan�c� ad�n�n minimum uzunluk kontrol�.
        RuleFor(p => p.EmailOrUserName)
             .MinimumLength(3)
             .WithMessage("Username or email must be at least 3 characters long.");

        // �ifrenin minimum uzunluk kontrol�.
        RuleFor(p => p.Password)
            .MinimumLength(1)
            .WithMessage("Password must be at least 1 character long.");
    }
}
