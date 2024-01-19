using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using SharedKernel.Application;
using SharedKernel.Auth;
using SharedKernel.MySQL;

namespace SharedKernel.SignalR
{
    public class MessageHub : Hub
    {
        public readonly IHubContext<MessageHub> _hubContext;
        public readonly IToken _token;
        public readonly IDbConnection _dbConnection;
        public readonly IMapper _mapper;
        public static Dictionary<string, List<string>> Connections = new Dictionary<string, List<string>>();
        public static Dictionary<string, string> KeyValueConnections = new Dictionary<string, string>();
        public static DateTime LastSendTyping = new DateTime();
        public static object lockObj = new object();

        public MessageHub(IHubContext<MessageHub> hubContext, IToken token, IDbConnection dbConnection, IMapper mapper)
        {
            _token = token;
            _dbConnection = dbConnection;
            _mapper = mapper;
            _hubContext = hubContext;
        }

        [HubMethodName("SendMessage")]
        public async Task SendMessageeee(string message)
        {
            await Clients.Caller.SendAsync("ReceiveMessage", message);
        }

        [HubMethodName("OnlineUsers")]
        public async Task SendNumberOfOnlineUsers()
        {
            await Clients.All.SendAsync("ReceiveMessage", new MessageHubResponse { Type = MessageHubType.OnlineUser, Message = KeyValueConnections.Count });
        }

        public async Task SomeOneTyping()
        {
            lock (lockObj)
            {
                if (DateTime.Now.Subtract(LastSendTyping).TotalSeconds > 1)
                {
                    LastSendTyping = DateTime.Now;
                    Clients.AllExcept(Context.ConnectionId).SendAsync("ReceiveMessage", new MessageHubResponse { Type = MessageHubType.SomeOneTyping });
                }
            }
            await Task.Yield();
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();

            var claimns = (Context.User.Identity as System.Security.Claims.ClaimsIdentity).Claims;
            if (!claimns.Any())
            {
                KeyValueConnections.TryAdd(Context.ConnectionId, Context.ConnectionId);
            }
            else
            {
                var tenant = claimns.First(x => x.Type == ClaimConstant.TENANT_ID);
                var owner = claimns.First(x => x.Type == ClaimConstant.USER_ID);
                var key = $"{tenant.Value}_{owner.Value}";

                if (Connections.TryGetValue(key, out var connectionIds))
                {
                    connectionIds.Add(Context.ConnectionId);
                    Connections[key] = connectionIds;
                }
                else
                {
                    Connections[key] = new List<string>
                    {
                        Context.ConnectionId
                    };
                }
                KeyValueConnections.TryAdd(Context.ConnectionId, key);
            }
            await SendNumberOfOnlineUsers();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
            if (KeyValueConnections.TryGetValue(Context.ConnectionId, out var value))
            {
                if (Connections.ContainsKey(value))
                {
                    Connections[value].Remove(Context.ConnectionId);
                }
                KeyValueConnections.Remove(Context.ConnectionId);
                await SendNumberOfOnlineUsers();
            }
        }
    }
}
