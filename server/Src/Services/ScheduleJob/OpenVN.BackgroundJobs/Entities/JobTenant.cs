using SharedKernel.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpenVN.BackgroundJobs
{
    [Table("common_tenant")]
    public class JobTenant : Tenant
    {
        [ForeignKey("TenantId")]
        public virtual List<JobUser> Users { get; set; }
    }
}
