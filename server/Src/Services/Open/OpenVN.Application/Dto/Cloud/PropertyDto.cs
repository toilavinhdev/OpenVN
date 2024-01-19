namespace OpenVN.Application
{
    public class PropertyDto
    {
        public string Name { get; set; }

        public long Size { get; set; }

        public string Type { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }
    }
}
