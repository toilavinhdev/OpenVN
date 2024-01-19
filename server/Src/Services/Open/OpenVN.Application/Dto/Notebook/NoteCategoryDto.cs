using FluentValidation;
using OpenVN.Application.Properties;
using Microsoft.Extensions.Localization;

namespace OpenVN.Application
{
    public class NoteCategoryDto
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public DateTime CreatedDate { get; set; }
    }

    public class NoteCategoryDtoValidator : AbstractValidator<NoteCategoryDto>
    {
        public NoteCategoryDtoValidator(IStringLocalizer<Resources> localizer)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizer["notecategory_name_must_not_be_empty"].Value);
        }
    }
}
