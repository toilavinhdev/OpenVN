using SharedKernel.Domain;

namespace OpenVN.Domain
{
    [Table("common_user_role")]
    public class UserRole : BaseEntity
    {
        public long RoleId { get; set; }

        public string UserId { get; set; }
    }
}
