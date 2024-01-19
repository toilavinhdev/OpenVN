namespace OpenVN.Domain
{
    [Table("cloud_directory")]
    public class Directory : PersonalizedEntity
    {
        public string Name { get; set; }

        public long ParentId { get; set; }

        public string Path { get; set; }

        public int DuplicateNo { get; set; }
    }
}
