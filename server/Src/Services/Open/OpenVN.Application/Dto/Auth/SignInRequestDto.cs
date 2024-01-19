using OpenVN.Application.Properties;
using Microsoft.Extensions.Localization;

namespace OpenVN.Application
{
    public class SignInRequestDto
    {
        public string UserName { get; set; }

        public string Password { get; set; }
    }

    public class SignInRequestDtoValidator : AbstractValidator<SignInRequestDto>
    {
        public SignInRequestDtoValidator(IStringLocalizer<Resources> localizer)
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage(localizer["auth_account_must_not_be_empty"].Value);
            RuleFor(x => x.Password).NotEmpty().WithMessage(localizer["auth_password_must_not_be_empty"].Value);
        }
    }
}
