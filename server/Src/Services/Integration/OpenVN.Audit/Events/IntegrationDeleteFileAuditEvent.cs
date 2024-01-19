using OpenVN.Audit.Entities;
using OpenVN.Audit.Models;

namespace OpenVN.Audit.Events
{
    public class IntegrationDeleteFileAuditEvent : IntegrationDeleteAuditEvent<CloudFile>
    {
        public List<DeleteFileAuditModel> Models { get; set; }
    }
}
