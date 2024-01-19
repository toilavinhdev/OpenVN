using OpenVN.Audit.Entities;
using OpenVN.Audit.Models;

namespace OpenVN.Audit.Events
{
    public class IntegrationUploadFileAuditEvent : IntegrationAuditEvent<CloudFile>
    {
        public List<UploadFileAuditModel> Models { get; set; }
    }
}
