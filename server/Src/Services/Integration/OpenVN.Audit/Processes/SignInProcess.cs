using OpenVN.Audit.Events;
using OpenVN.Audit.Models;
using SharedKernel.Domain;

namespace OpenVN.Audit.Processes
{
    public class SignInProcess : BaseProcess<BaseEntity>
    {
        public SignInProcess() : base(default)
        {
        }

        protected override List<AuditEntity> GetParameter(IntegrationAuditEvent<BaseEntity> auditEvent, string bodyStr)
        {
            return new List<AuditEntity> { CreateBaseAuditEntity(auditEvent, "<p>Đăng nhập thành công</p>") };
        }
    }
}
