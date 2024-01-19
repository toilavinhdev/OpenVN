using SharedKernel.Domain;

namespace OpenVN.Audit.Events
{
    public class IntegrationUpdateFavouriteAuditEvent : IntegrationAuditEvent<BaseEntity>
    {
        public string AppName { get; set; }

        public bool IsFavourite { get; set; }
    }
}
