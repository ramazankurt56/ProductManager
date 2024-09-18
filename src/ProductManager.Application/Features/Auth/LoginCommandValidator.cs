using FluentValidation;

namespace ProductManager.Application.Features.Auth;

/// <summary>
/// LoginCommand için validation  kurallarýný tanýmlar.
/// </summary>
public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    /// <summary>
    /// Yeni bir instance oluþturur ve validation kurallarýný tanýmlar.
    /// </summary>
    public LoginCommandValidator()
    {
        // Email veya kullanýcý adýnýn minimum uzunluk kontrolü.
        RuleFor(p => p.EmailOrUserName)
             .MinimumLength(3)
             .WithMessage("Username or email must be at least 3 characters long.");

        // Þifrenin minimum uzunluk kontrolü.
        RuleFor(p => p.Password)
            .MinimumLength(1)
            .WithMessage("Password must be at least 1 character long.");
    }
}
