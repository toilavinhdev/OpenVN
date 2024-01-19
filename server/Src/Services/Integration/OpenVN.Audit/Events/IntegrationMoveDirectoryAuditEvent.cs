using OpenVN.Audit.Models;

namespace OpenVN.Audit.Events
{
    public class IntegrationMoveDirectoryAuditEvent : IntegrationAuditEvent<Directory>
    {
        public List<MoveDirectoryAuditModel> Models { get; set; }
    }
}
