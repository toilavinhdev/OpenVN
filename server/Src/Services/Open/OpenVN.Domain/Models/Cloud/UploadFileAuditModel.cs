namespace OpenVN.Domain
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
