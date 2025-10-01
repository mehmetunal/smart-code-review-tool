using FluentValidation;
using SmartCodeReview.Dto.Mssql.Auth;

namespace SmartCodeReview.Dto.Mssql.Validators;

/// <summary>
/// RegisterDto validator
/// </summary>
public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email adresi gereklidir")
            .EmailAddress().WithMessage("Geçerli bir email adresi giriniz")
            .MaximumLength(256).WithMessage("Email adresi en fazla 256 karakter olabilir");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre gereklidir")
            .MinimumLength(8).WithMessage("Şifre en az 8 karakter olmalıdır")
            .Matches(@"[A-Z]").WithMessage("Şifre en az bir büyük harf içermelidir")
            .Matches(@"[a-z]").WithMessage("Şifre en az bir küçük harf içermelidir")
            .Matches(@"[0-9]").WithMessage("Şifre en az bir rakam içermelidir")
            .Matches(@"[\!\@\#\$\%\^\&\*]").WithMessage("Şifre en az bir özel karakter içermelidir");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Ad gereklidir")
            .MaximumLength(100).WithMessage("Ad en fazla 100 karakter olabilir");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Soyad gereklidir")
            .MaximumLength(100).WithMessage("Soyad en fazla 100 karakter olabilir");
    }
}

