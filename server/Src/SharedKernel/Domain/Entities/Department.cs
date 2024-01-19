using System.ComponentModel.DataAnnotations.Schema;

namespace SharedKernel.Domain
{
    [Table("common_department")]
    public class Department : BaseEntity
    {
        public string Name { get; set; }

        public long BranchId { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
