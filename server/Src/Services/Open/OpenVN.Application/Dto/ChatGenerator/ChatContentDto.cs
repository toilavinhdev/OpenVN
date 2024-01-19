namespace OpenVN.Application
{
    public class ChatContentDto
    {
        public List<ChatUserDto> Contents { get; set; }
    }

    public class ChatUserDto
    {
        public string Key { get; set; }
        public string Content { get; set; }
        public int At { get; set; }
    }
}
