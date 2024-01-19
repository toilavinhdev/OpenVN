using Newtonsoft.Json;
using OpenVN.Audit.Entities;
using OpenVN.Audit.Events;
using OpenVN.Audit.Models;
using SharedKernel.Domain;

namespace OpenVN.Audit.Processes
{
    public class FileProcess : BaseProcess<CloudFile>
    {
        public FileProcess() : this(new AuditConfigModel("tệp"))
        {
        }

        public FileProcess(AuditConfigModel config) : base(config)
        {
        }

        protected override List<AuditEntity> GetDeleteParameter(string bodyStr, string[] ignoreFields)
        {
            var @event = JsonConvert.DeserializeObject<IntegrationDeleteFileAuditEvent>(bodyStr);
            var result = new List<AuditEntity>();

            foreach (var model in @event.Models)
            {
                var description = string.Empty;
                if (model.Directory != null)
                {
                    description = $"<p>Tệp <strong>{model.File.OriginalFileName}</strong> bị xóa theo thư mục gốc <strong>{model.Directory.Name}</strong><p>";
                }
                else
                {
                    description = $"<p>Xóa tệp <strong>{model.File.OriginalFileName}</strong><p>";
                }
                result.Add(CreateBaseAuditEntity(@event, description));
            }
            return result;
        }

        protected override List<AuditEntity> GetCustomParameter(IntegrationAuditEvent<CloudFile> auditEvent, string bodyStr, string[] ignoreFields)
        {
            var result = new List<AuditEntity>();
            switch (auditEvent.AuditAction)
            {
                case AuditAction.Upload:
                    var @event = JsonConvert.DeserializeObject<IntegrationUploadFileAuditEvent>(bodyStr);
                    foreach (var model in @event.Models)
                    {
                        var description = string.Empty;
                        if (model.Directory != null)
                        {
                            description = $"<p>Tải lên tệp <strong>{model.File.OriginalFileName}</strong> vào thư mục <strong>{model.Directory.Name}</strong><p>";
                        }
                        else
                        {
                            description = $"<p>Tải lên tệp <strong>{model.File.OriginalFileName}</strong><p>";
                        }
                        result.Add(CreateBaseAuditEntity(@event, description));
                    }
                    return result;
                case AuditAction.Move:
                    var @event2 = JsonConvert.DeserializeObject<IntegrationMoveFileAuditEvent>(bodyStr);
                    foreach (var model in @event2.Models)
                    {
                        var description = string.Empty;
                        var sourceName = "C:/";
                        var destinationName = "C:/";

                        if (model.Source != null)
                        {
                            sourceName = model.Source.Name;
                        }
                        if (model.Destination != null)
                        {
                            destinationName = model.Destination.Name;
                        }

                        description = $"<p>Tệp <strong>{model.MovedFile.OriginalFileName}</strong> được chuyển từ <strong>{sourceName}</strong> vào trong <strong>{destinationName}</strong><p>";
                        result.Add(CreateBaseAuditEntity(@event2, description));
                    }
                    return result;
            }
            return base.GetCustomParameter(auditEvent, bodyStr, ignoreFields);
        }
    }
}
