namespace OpenVN.Domain
{
    public class DeleteDirectoryAuditModel
    {
        public Directory Directory { get; }
        public Directory Parent { get; }
        public bool IsDeletedByRoot { get; }

        public DeleteDirectoryAuditModel(Directory directory, Directory parent, bool isDeletedByRoot = false)
        {
            Directory = directory;
            Parent = parent;
            IsDeletedByRoot = isDeletedByRoot;
        }
    }
}
