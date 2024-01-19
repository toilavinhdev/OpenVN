using SharedKernel.Domain;

namespace OpenVN.Audit.Events
{
    public class IntegrationUpdateAuditEvent<T> : IntegrationAuditEvent<T> where T : IBaseEntity
    {
    }
}
