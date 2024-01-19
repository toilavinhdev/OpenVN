using Microsoft.Extensions.Localization;
using OpenVN.Application.Properties;

namespace OpenVN.Application
{
    public class SignInCommand : BaseAllowAnonymousCommand<BaseResponse>
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public SignInCommand(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }
    }

    public class SignInCommandValidator : AbstractValidator<SignInRequestDto>
    {
        public SignInCommandValidator(IStringLocalizer<Resources> localizer)
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage(localizer["auth_account_must_not_be_empty"].Value);
            RuleFor(x => x.Password).NotEmpty().WithMessage(localizer["auth_password_must_not_be_empty"].Value);
        }
    }
}
