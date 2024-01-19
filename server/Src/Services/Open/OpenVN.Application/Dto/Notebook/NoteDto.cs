using OpenVN.Application.Properties;
using Microsoft.Extensions.Localization;

namespace OpenVN.Application
{
    public class NoteDto : NoteWithoutContentDto
    {
        public string Content { get; set; }

        public string OwnerId { get; set; }
    }

    public class NoteDtoValidator : AbstractValidator<NoteDto>
    {
        public NoteDtoValidator(IStringLocalizer<Resources> localizer)
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage(localizer["note_title_must_not_be_empty"].Value);
            RuleFor(x => x.Content).NotEmpty().WithMessage(localizer["note_content_must_not_be_empty"].Value);
            RuleFor(x => x.CategoryId).NotEmpty().WithMessage(localizer["note_category_id_must_not_be_empty"].Value);
        }
    }
}
