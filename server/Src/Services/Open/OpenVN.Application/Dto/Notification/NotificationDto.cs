namespace OpenVN.Application
{
    public class NotificationDto
    {
        public string Id { get; set; }

        public NotificationType Type { get; set; }

        public bool IsUnread { get; set; } = true;

        public string Description { get; set; }

        private DateTime _timestamp;

        public DateTime Timestamp
        {
            get
            {
                return _timestamp.AddHours(7);
            }
            set
            {
                _timestamp = value;
            }
        }

        public DateTime CreatedDate { get; set; }
    }
}
