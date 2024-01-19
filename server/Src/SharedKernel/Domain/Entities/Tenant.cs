using SharedKernel.Libraries;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using KeyAttribute = System.ComponentModel.DataAnnotations.KeyAttribute;

namespace SharedKernel.Domain
{
    [Table("common_tenant")]
    public class Tenant : CoreEntity, IAuditable
    {
        [Key]
        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        [JsonIgnore]
        public bool IsDeleted { get; set; }

        public DateTime CreatedDate { get; set; } = DateHelper.Now;

        public long CreatedBy { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public long? LastModifiedBy { get; set; }

        public DateTime? DeletedDate { get; set; }

        public long? DeletedBy { get; set; }
    }
}
