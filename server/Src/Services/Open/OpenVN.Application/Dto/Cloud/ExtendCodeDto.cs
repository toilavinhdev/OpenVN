using Microsoft.Extensions.Localization;
using OpenVN.Application.Properties;

namespace OpenVN.Application
{
    public class ExtendCodeDto
    {
        public string DirectoryId { get; set; }

        public string Code { get; set; }
    }

    public class ExtendCodeDtoDtoValidator : AbstractValidator<ExtendCodeDto>
    {
        public ExtendCodeDtoDtoValidator(IStringLocalizer<Resources> localizer)
        {
            RuleFor(x => x.Code).NotEmpty().WithMessage(localizer["cloud_directory_password_is_incorrect"]);
        }
    }
}
