namespace OpenVN.Domain
{
    [Table("common_role_action")]
    public class RoleAction : BaseEntity
    {
        public int RoleId { get; set; }

        public string RoleCode { get; set; }

        public string RoleName { get; set; }

        public long ActionId { get; set; }

        public string ActionCode { get; set; }

        public string ActionName { get; set; }

        public string Description { get; set; }

        public int Exponent { get; set; }
    }
}
