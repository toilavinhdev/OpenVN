using FluentValidation;
using OpenVN.Application.Properties;
using Microsoft.Extensions.Localization;
using SharedKernel.Libraries;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    public class CreateUserDto
    {
        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string ConfirmPassword { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string FullName => $"{FirstName} {LastName}";

        public string Address { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        public GenderType Gender { get; set; }

        public string TenantId { get; set; }
    }

    public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserDtoValidator(IStringLocalizer<Resources> localizer)
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage(localizer["auth_account_must_not_be_empty"].Value);
            RuleFor(x => x.Password).NotEmpty().WithMessage(localizer["auth_password_must_not_be_empty"].Value);
            RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage(localizer["auth_confirm_password_must_not_be_empty"].Value);
            RuleFor(x => x).Must(x => x.Password.Equals(x.ConfirmPassword)).WithMessage(localizer["auth_pwd_n_cpwd_must_same"].Value);
            RuleFor(x => x.Email).NotEmpty().WithMessage(localizer["auth_email_must_not_be_empty"].Value);
            //RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage(localizer["auth_phone_must_not_be_empty"].Value); // NEED_REFACTOR
            RuleFor(x => x).Must(x => Utility.IsEmail(x.Email)).WithMessage(localizer["auth_email_is_invalid"].Value);
            //RuleFor(x => x).Must(x => Utility.IsPhoneNumber(x.PhoneNumber)).WithMessage(localizer["auth_phone_is_invalid"].Value); // NEED_REFACTOR
        }
    }
}
