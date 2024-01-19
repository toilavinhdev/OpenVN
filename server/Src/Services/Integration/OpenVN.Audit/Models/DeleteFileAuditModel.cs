using OpenVN.Audit.Entities;

namespace OpenVN.Audit.Models;

public class DeleteFileAuditModel
{
    public Directory Directory { get; }
    public CloudFile File { get; }
    public DeleteFileAuditModel(CloudFile file, Directory directory = default)
    {
        Directory = directory;
        File = file;
    }
}
