using OpenVN.Audit.Models;

namespace OpenVN.Audit.Events
{
    public class IntegrationDeleteDirectoryAuditEvent : IntegrationDeleteAuditEvent<Directory>
    {
        public List<DeleteDirectoryAuditModel> Models { get; set; }
    }
}
