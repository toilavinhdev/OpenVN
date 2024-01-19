using OpenVN.Audit.Entities;

namespace OpenVN.Audit.Models
{
    public class MoveFileAuditModel
    {
        public CloudFile MovedFile { get; }
        public Directory Source { get; }
        public Directory Destination { get; }

        public MoveFileAuditModel(CloudFile movedFile, Directory source, Directory destination)
        {
            MovedFile = movedFile;
            Source = source;
            Destination = destination;
        }
    }
}
