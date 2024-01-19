using Microsoft.Extensions.Localization;
using OpenVN.Application.Properties;

namespace OpenVN.Application
{
    public class MoveDto
    {
        public string DestinationId { get; set; }

        public string DestinationSecretCode { get; set; }

        public string SourceSecretCode { get; set; }

        public List<MoveObject> MoveObjects { get; set; }
    }

    public class MoveObject
    {
        public string Type { get; set; }

        public string SourceId { get; set; }
    }

    public class MoveDtoValidator : AbstractValidator<MoveDto>
    {
        public MoveDtoValidator(IStringLocalizer<Resources> localizer)
        {
            RuleFor(x => x.DestinationId)
                .Must(x => long.TryParse(x, out var id) && id >= 0)
                .WithMessage(localizer["cloud_destination_id_is_invalid"]);

            RuleFor(x => x.MoveObjects).NotEmpty();
            RuleForEach(x => x.MoveObjects).SetValidator(new MoveObjectValidator(localizer));
        }
    }

    public class MoveObjectValidator : AbstractValidator<MoveObject>
    {

        public MoveObjectValidator(IStringLocalizer<Resources> localizer)
        {
            RuleFor(x => x.Type)
                .Must(type => new string[] { "dir", "cf" }.Contains(type?.ToLower()))
                .WithMessage(localizer["cloud_move_object_type_is_invalid"]);

            RuleFor(x => x.SourceId)
                .Must(x => long.TryParse(x, out var id) && id >= 0)
                .WithMessage(localizer["cloud_move_object_id_is_invalid"]);
        }
    }
}
