using Newtonsoft.Json;
using OpenVN.Audit.Entities;
using OpenVN.Audit.Events;
using SharedKernel.Domain;

namespace OpenVN.Audit.Processes
{
    public class AvatarProcess : BaseProcess<Avatar>
    {
        public AvatarProcess() : base(new Models.AuditConfigModel("ảnh đại diện"))
        {
        }

        protected override List<AuditEntity> GetParameter(IntegrationAuditEvent<Avatar> auditEvent, string bodyStr)
        {
            var @event = JsonConvert.DeserializeObject<IntegrationUpdateAvatarAuditEvent>(bodyStr);
            var description = "<p>Cập nhật ảnh đại diện</p>";

            if (@event.IsRemoved)
            {
                description = "<p>Xóa ảnh đại diện</p>";
            }
            return new List<AuditEntity> { CreateBaseAuditEntity(auditEvent, description) };
        }
    }
}
