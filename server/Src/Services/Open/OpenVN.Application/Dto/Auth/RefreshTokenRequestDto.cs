using FluentValidation;
using OpenVN.Application.Properties;
using Microsoft.Extensions.Localization;

namespace OpenVN.Application
{
    public class RefreshTokenRequestDto
    {
        public long UserId { get; set; }

        public string RefreshToken { get; set; }
    }

    public class RefreshTokenRequestDtoValidator : AbstractValidator<RefreshTokenRequestDto>
    {
        public RefreshTokenRequestDtoValidator(IStringLocalizer<Resources> localizer)
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage(localizer["auth_userid_must_not_be_empty"].Value);
            RuleFor(x => x.RefreshToken).NotEmpty().WithMessage(localizer["auth_refresh_token_must_not_be_empty"].Value);
        }
    }
}
