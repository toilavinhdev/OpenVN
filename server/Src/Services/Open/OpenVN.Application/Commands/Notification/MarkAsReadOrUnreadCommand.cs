namespace OpenVN.Application
{
    public class MarkAsReadOrUnreadCommand : BaseUpdateCommand
    {
        public string Id { get; }
        public bool MarkAsRead { get; }

        public MarkAsReadOrUnreadCommand(string id, bool markAsRead = true)
        {
            Id = id;
            MarkAsRead = markAsRead;
        }
    }
}
