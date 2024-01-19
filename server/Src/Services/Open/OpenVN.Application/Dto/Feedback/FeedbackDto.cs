using Microsoft.Extensions.Localization;
using SharedKernel.Properties;

namespace OpenVN.Application
{
    public class FeedbackDto
    {
        public string Id { get; set; }

        public long ParentId { get; set; }

        public string Content { get; set; }

        public string OwnerId { get; set; }

        public DateTime CreatedDate { get; set; }

        public string AvatarUrl { get; set; }

        public string Email { get; set; }

        public bool HasReply => ReplyCount > 0;

        public int ReplyCount { get; set; }

        public string FromIP { get; set; }

        public string TenantId { get; set; }
    }

    public class FeedbackDtoValidator : AbstractValidator<FeedbackDto>
    {
        public FeedbackDtoValidator(IStringLocalizer<Resources> localizer)
        {
            RuleFor(x => x.Content).NotEmpty().WithMessage(localizer["bad_data"]);
        }
    }
}
