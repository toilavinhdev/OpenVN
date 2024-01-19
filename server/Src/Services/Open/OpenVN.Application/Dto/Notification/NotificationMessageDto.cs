using SharedKernel.SignalR;

namespace OpenVN.Application
{
    public class NotificationMessageDto
    {
        public string Id { get; set; }

        public MessageHubType Type { get; set; }

        public string Description { get; set; }

        public bool IsAllClients { get; set; }

        public List<string> Keys { get; set; }
    }
}
