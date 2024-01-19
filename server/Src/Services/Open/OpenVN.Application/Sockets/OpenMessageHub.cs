using Microsoft.AspNetCore.SignalR;
using SharedKernel.Auth;
using SharedKernel.Libraries;
using SharedKernel.Log;
using SharedKernel.SignalR;

namespace OpenVN.Application
{
    public class OpenMessageHub : IOpenMessageHub
    {
        private readonly IHubContext<MessageHub> _hubContext;
        private readonly IToken _token;

        public OpenMessageHub(IHubContext<MessageHub> hubContext, IToken token)
        {
            _hubContext = hubContext;
            _token = token;
        }

        public IHubContext<MessageHub> HubContext => _hubContext;

        public async Task SendMessages(NotificationMessageDto notification, CancellationToken cancellationToken = default)
        {
            try
            {
                if (notification == null)
                {
                    return;
                }

                if (notification.IsAllClients)
                {
                    await _hubContext.Clients.All.SendAsync("ReceiveMessage", new MessageHubResponse { Type = notification.Type, Message = notification.Description });
                    return;
                }
                foreach (var key in notification.Keys)
                {
                    if (MessageHub.Connections.TryGetValue(key, out var connectionIds))
                    {
                        await _hubContext.Clients.Clients(connectionIds).SendAsync("ReceiveMessage", new MessageHubResponse { Type = notification.Type, Message = notification.Description });
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Error(ex);
                throw;
            }
        }

        public async Task SendSignInMessage(CancellationToken cancellationToken = default)
        {
            var key = $"{_token.Context.TenantId}_{_token.Context.OwnerId}";
            if (MessageHub.Connections.TryGetValue(key, out var connectionIds))
            {
                var ip = AuthUtility.TryGetIP(_token.Context.HttpContext.Request);
                await _hubContext.Clients.Clients(connectionIds).SendAsync("ReceiveMessage", new MessageHubResponse { Type = MessageHubType.SignIn, Message = ip });
            }
        }

        public async Task SendSignOutMessage(CancellationToken cancellationToken = default)
        {
            var key = $"{_token.Context.TenantId}_{_token.Context.OwnerId}";
            if (MessageHub.Connections.TryGetValue(key, out var connectionIds))
            {
                await _hubContext.Clients.Clients(connectionIds).SendAsync("ReceiveMessage", new MessageHubResponse { Type = MessageHubType.SignOut });
                MessageHub.Connections.Remove(key);
            }
        }
    }
}
