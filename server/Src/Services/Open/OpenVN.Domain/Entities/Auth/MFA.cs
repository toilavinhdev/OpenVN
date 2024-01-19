using FluentValidation;
using static SharedKernel.Application.Enum;

namespace OpenVN.Domain
{
    [Table("auth_mfa")]
    public class MFA : BaseEntity
    {
        public string UserId { get; set; }

        public MFAType Type { get; set; } = MFAType.None;

        public bool Enabled { get; set; }
    }

    public class MFAValidator : AbstractValidator<MFA>
    {
        public MFAValidator()
        {
            RuleFor(x => x.Type).NotEmpty();
        }
    }
}
