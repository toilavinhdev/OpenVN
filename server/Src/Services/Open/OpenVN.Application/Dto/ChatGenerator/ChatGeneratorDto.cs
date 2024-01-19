namespace OpenVN.Application
{
    public class ChatGeneratorDto
    {
        public string Id { get; set; }

        public string FileName { get; set; }

        public ChatContentDto Content { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
