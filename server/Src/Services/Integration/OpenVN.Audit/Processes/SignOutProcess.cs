using OpenVN.Audit.Events;
using SharedKernel.Domain;

namespace OpenVN.Audit.Processes
{
    public class SignOutProcess : BaseProcess<BaseEntity>
    {
        public SignOutProcess() : base(default)
        {
        }

        protected override List<AuditEntity> GetParameter(IntegrationAuditEvent<BaseEntity> auditEvent, string bodyStr)
        {
            return new List<AuditEntity> { CreateBaseAuditEntity(auditEvent, "<p>Đăng xuất thành công</p>") };
        }
    }
}
