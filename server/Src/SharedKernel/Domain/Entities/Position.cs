using System.ComponentModel.DataAnnotations.Schema;

namespace SharedKernel.Domain
{
    [Table("common_position")]
    public class Position : BaseEntity
    {
        public string Name { get; set; }
    }
}
