using Microsoft.Extensions.Localization;
using OpenVN.Application.Properties;
using System.Text.Json.Serialization;

namespace OpenVN.Application
{
    public class DirectoryDto
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string ParentId { get; set; }

        public List<DirectoryDto> Children { get; set; } = new();

        public int ChildrenCount { get; set; }

        public string Path { get; set; }

        public bool IsLocked { get; set; }

        public bool HasPassword { get; set; }

        [JsonIgnore]
        public DateTime CreatedDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }
    }

    public class DirectoryDtoValidator : AbstractValidator<DirectoryDto>
    {
        public DirectoryDtoValidator(IStringLocalizer<Resources> localizer)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizer["directory_name_must_not_be_empty"]);
            RuleFor(x => x.ParentId).Must(p => long.TryParse(p, out var id) && id >= 0).WithMessage(localizer["directory_parent_id_is_invalid"]);
        }
    }
}
