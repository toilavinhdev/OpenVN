using Newtonsoft.Json;
using OpenVN.Audit.Events;
using OpenVN.Audit.Models;
using SharedKernel.Domain;

namespace OpenVN.Audit.Processes
{
    public class AppProcess : BaseProcess<BaseEntity>
    {
        public AppProcess() : base(new AuditConfigModel("app"))
        {
        }

        protected override List<AuditEntity> GetCustomParameter(IntegrationAuditEvent<BaseEntity> auditEvent, string bodyStr, string[] ignoreFields)
        {
            var @event = JsonConvert.DeserializeObject<IntegrationUpdateFavouriteAuditEvent>(bodyStr);
            switch (@event.AuditAction)
            {
                case AuditAction.AppFavourite:
                    var message = @event.IsFavourite ? 
                        $"Thêm <strong>{@event.AppName}</strong> vào danh sách yêu thích" : 
                        $"Loại bỏ <strong>{@event.AppName}</strong> khỏi danh sách yêu thích";
                    return new List<AuditEntity> { CreateBaseAuditEntity(@event, message) };
                default:
                    return base.GetCustomParameter(auditEvent, bodyStr, ignoreFields);
            }
        }
    }
}
