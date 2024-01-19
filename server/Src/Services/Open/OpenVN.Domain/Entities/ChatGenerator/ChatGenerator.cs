namespace OpenVN.Domain
{
    [Table("chat_generator")]
    public class ChatGenerator : PersonalizedEntity
    {
        public string FileName { get; set; }

        public string Contents { get; set; }
    }
}
