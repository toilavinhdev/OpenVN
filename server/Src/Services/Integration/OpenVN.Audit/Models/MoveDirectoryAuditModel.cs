namespace OpenVN.Audit.Models
{
    public class MoveDirectoryAuditModel
    {
        public Directory MovedDirectory { get; }

        public Directory Source { get; }

        public Directory Destination { get; }

        public MoveDirectoryAuditModel(Directory movedDirectory, Directory source, Directory destination)
        {
            MovedDirectory = movedDirectory;
            Source = source;
            Destination = destination;
        }
    }
}
