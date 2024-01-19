namespace OpenVN.Domain
{
    [Table("common_avatar")]
    public class Avatar : PersonalizedEntity
    {
        public string FileName { get; set; }
    }
}
