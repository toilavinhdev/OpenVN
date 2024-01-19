namespace OpenVN.Domain
{
    public class Note : PersonalizedEntity
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public int Order { get; set; }

        public long CategoryId { get; set; }

        public bool IsPublic { get; set; }

        public bool IsPinned { get; set; }

        public string BackgroundColor { get; set; }
    }
}
