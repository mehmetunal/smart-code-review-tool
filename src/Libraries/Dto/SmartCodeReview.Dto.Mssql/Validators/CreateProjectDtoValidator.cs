using FluentValidation;
using SmartCodeReview.Dto.Mssql.Project;

namespace SmartCodeReview.Dto.Mssql.Validators;

/// <summary>
/// CreateProjectDto validator
/// </summary>
public class CreateProjectDtoValidator : AbstractValidator<CreateProjectDto>
{
    public CreateProjectDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Proje adı gereklidir")
            .MaximumLength(200).WithMessage("Proje adı en fazla 200 karakter olabilir");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Açıklama en fazla 1000 karakter olabilir");

        RuleFor(x => x.RepositoryUrl)
            .NotEmpty().WithMessage("Repository URL gereklidir")
            .Must(BeValidUrl).WithMessage("Geçerli bir URL giriniz")
            .MaximumLength(500).WithMessage("URL en fazla 500 karakter olabilir");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Repository full name gereklidir (owner/repo)")
            .Matches(@"^[\w-]+\/[\w-]+$").WithMessage("Geçerli format: owner/repo")
            .MaximumLength(200).WithMessage("Full name en fazla 200 karakter olabilir");
    }

    private bool BeValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}

