using OpenVN.Audit.Entities;

namespace OpenVN.Audit.Models
{
    public class UploadFileAuditModel
    {
        public CloudFile File { get; }
        public Directory Directory { get; }

        public UploadFileAuditModel(CloudFile file, Directory directory = default)
        {
            File = file;
            Directory = directory;
        }
    }
}
