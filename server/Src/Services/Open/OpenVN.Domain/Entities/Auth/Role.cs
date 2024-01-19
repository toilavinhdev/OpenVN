using SharedKernel.Domain;

namespace OpenVN.Domain
{
    [Table("common_role")]
    public class Role : BaseEntity
    {
        public string Code { get; set; }

        public string Name { get; set; }
    }
}
