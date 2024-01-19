using Microsoft.Extensions.Localization;
using OpenVN.Application.Properties;

namespace OpenVN.Application
{
    public class UnlockDirectoryDto
    {
        public string DirectoryId { get; set; }

        public string Password { get; set; }
    }

    public class UnlockDirectoryDtoValidator : AbstractValidator<UnlockDirectoryDto>
    {
        public UnlockDirectoryDtoValidator(IStringLocalizer<Resources> localizer)
        {
            RuleFor(x => x.Password).NotEmpty().WithMessage(localizer["cloud_directory_password_is_incorrect"]);
        }
    }
}
