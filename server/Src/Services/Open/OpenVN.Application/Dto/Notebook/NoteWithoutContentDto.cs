namespace OpenVN.Application
{
    public class NoteWithoutContentDto
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public int Order { get; set; }

        public string CategoryId { get; set; }

        public string CategoryName { get; set; }

        public bool IsPublic { get; set; }

        public bool IsPinned { get; set; }

        public string BackgroundColor { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }
    }
}
