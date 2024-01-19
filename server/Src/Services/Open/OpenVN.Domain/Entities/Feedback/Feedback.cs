namespace OpenVN.Domain
{
    [Table("feedback")]
    public class Feedback : PersonalizedEntity
    {
        public long ParentId { get; set; }

        public string Content { get; set; }

        public string Path { get; set; }

        public string FromIP { get; set; }
    }
}
