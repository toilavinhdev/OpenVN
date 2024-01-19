namespace OpenVN.Domain
{
    [Table("cloud_locked_directory")]
    public class LockedDirectory : PersonalizedEntity
    {
        public long DirectoryId { get; set; }

        public bool EnabledLock { get; set; }

        public string Password { get; set; }
    }
}
