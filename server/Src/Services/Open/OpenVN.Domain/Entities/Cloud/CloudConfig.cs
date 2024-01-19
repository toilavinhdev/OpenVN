namespace OpenVN.Domain
{
    [Table("cloud_config")]
    public class CloudConfig : PersonalizedEntity
    {
        public long MaxCapacity { get; set; }
        public long MaxFileSize { get; set; }
    }
}
