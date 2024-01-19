namespace OpenVN.Application
{
    public class SendMessageCommand : BaseCommand
    {
        public NotificationMessageDto Notification { get; }

        public SendMessageCommand(NotificationMessageDto notification)
        {
            Notification = notification;
        }
    }
}
