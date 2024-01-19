using Microsoft.AspNetCore.SignalR;
using SharedKernel.SignalR;

namespace OpenVN.Application
{
    public interface IOpenMessageHub
    {
        IHubContext<MessageHub> HubContext { get; }

        Task SendMessages(NotificationMessageDto notification, CancellationToken cancellationToken = default);

        Task SendSignInMessage(CancellationToken cancellationToken = default);

        Task SendSignOutMessage(CancellationToken cancellationToken = default);
    }
}
