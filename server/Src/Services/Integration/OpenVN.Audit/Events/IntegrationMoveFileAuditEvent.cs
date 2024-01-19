using OpenVN.Audit.Entities;
using OpenVN.Audit.Models;

namespace OpenVN.Audit.Events
{
    public class IntegrationMoveFileAuditEvent : IntegrationAuditEvent<CloudFile>
    {
        public List<MoveFileAuditModel> Models { get; set; }
    }
}
