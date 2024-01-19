using SharedKernel.Auth;
using SharedKernel.Domain;

namespace OpenVN.Master.Domain.Events
{
    public class UpdateFavouriteAuditEvent : AuditEvent
    {
        public string AppName { get; }

        public bool IsFavourite { get; }

        public UpdateFavouriteAuditEvent(string appName, bool isFavourite, IToken token, Guid eventId = default) : base("AppMaster", AuditAction.AppFavourite, token, eventId)
        {
            AppName = appName;
            IsFavourite = isFavourite;
        }
    }
}
