using SharedKernel.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using static SharedKernel.Application.Enum;

namespace OpenVN.BackgroundJobs
{
    [Table("common_user")]
    public class JobUser : IAuditable
    {
        [Key]
        public long Id { get; set; }

        public string Username { get; set; }

        public string PasswordHash { get; set; }

        public string Salt { get; set; }

        public string PhoneNumber { get; set; }

        public bool ConfirmedPhone { get; set; }

        public string Email { get; set; }

        public bool ConfirmedEmail { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; }

        public DateTime DateOfBirth { get; set; }

        public GenderType Gender { get; set; }

        public long PositionId { get; set; }

        public long DepartmentId { get; set; }

        [JsonIgnore]
        public bool IsDeleted { get; set; }

        public DateTime CreatedDate { get; set; } = SharedKernel.Libraries.DateHelper.Now;

        public long CreatedBy { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public long? LastModifiedBy { get; set; }

        public DateTime? DeletedDate { get; set; }

        public long? DeletedBy { get; set; }

        public virtual JobTenant Tenant { get; set; }

        [ForeignKey("OwnerId")]
        public virtual List<Process> Processes { get; set; }
    }
}
