using Newtonsoft.Json;
using OpenVN.Audit.Events;
using OpenVN.Audit.Models;
using SharedKernel.Domain;

namespace OpenVN.Audit.Processes
{
    public class DirectoryProcess : BaseProcess<Directory>
    {
        public DirectoryProcess() : this(new AuditConfigModel("thư mục"))
        {
        }

        public DirectoryProcess(AuditConfigModel config) : base(config)
        {
        }

        protected override List<AuditEntity> GetDeleteParameter(string bodyStr, string[] ignoreFields)
        {
            var @event = JsonConvert.DeserializeObject<IntegrationDeleteDirectoryAuditEvent>(bodyStr);
            var result = new List<AuditEntity>();

            foreach (var model in @event.Models)
            {
                result.Add(CreateBaseAuditEntity(@event, GetDeleteDirectoryDescription(model)));
            }
            return result;
        }

        protected override List<AuditEntity> GetCustomParameter(IntegrationAuditEvent<Directory> auditEvent, string bodyStr, string[] ignoreFields)
        {
            switch (auditEvent.AuditAction)
            {
                case AuditAction.Move:
                    var @event = JsonConvert.DeserializeObject<IntegrationMoveDirectoryAuditEvent>(bodyStr);
                    var result = new List<AuditEntity>();

                    foreach (var model in @event.Models)
                    {
                        var sourceName = model.Source != null ? model.Source.Name : "C:/";
                        var destinationName = model.Destination != null ? model.Destination.Name : "C:/";
                        var description = $"<p>Thư mục <strong>{model.MovedDirectory.Name}</strong> được chuyển từ <strong>{sourceName}</strong> vào trong <strong>{destinationName}</strong><p>";

                        result.Add(CreateBaseAuditEntity(@event, description));
                    }
                    return result;                    
                default:
                    return base.GetCustomParameter(auditEvent, bodyStr, ignoreFields);
            }
        }

        private string GetDeleteDirectoryDescription(DeleteDirectoryAuditModel model)
        {
            if (model.IsDeletedByRoot)
            {
                return $"<p>Thư mục <strong>{model.Directory.Name}</strong> bị xóa theo thư mục gốc <strong>{model.Parent.Name}</strong><p>";
            }
            return $"<p>Xóa thư mục <strong>{model.Directory.Name}</strong><p>";
        }
    }
}
