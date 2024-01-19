using System.ComponentModel.DataAnnotations.Schema;

namespace SharedKernel.Domain
{
    [Table("common_branch")]
    public class Branch : BaseEntity
    {
        public string Name { get; set; }

        public string Address { get; set; }
    }
}
