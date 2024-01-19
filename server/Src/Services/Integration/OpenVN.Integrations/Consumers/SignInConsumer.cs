using Newtonsoft.Json;
using OpenVN.Integrations.Models.Notification;
using RabbitMQ.Client;
using SharedKernel.Application;
using SharedKernel.Caching;
using SharedKernel.Domain;
using SharedKernel.Libraries;
using SharedKernel.Log;
using SharedKernel.MySQL;
using SharedKernel.Runtime.Exceptions;
using SharedKernel.SignalR;
using System.Text;
using System.Threading;

namespace OpenVN.BackgroundJob
{
    public class SignInConsumer : DefaultBasicConsumer
    {
        private readonly IServiceProvider _provider;
        private readonly IModel _channel;
        private readonly IIntegrationAuthNoticeService _authNoticeService;
        private readonly IIntegrationAuthRepository _authRepository;
        private readonly ISequenceCaching _sequenceCaching;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _socketServiceUrl;

        public SignInConsumer(IServiceProvider provider, IModel channel)
        {
            _provider = provider;
            _channel = channel;
            _authNoticeService = _provider.GetRequiredService<IIntegrationAuthNoticeService>();
            _authRepository = _provider.GetRequiredService<IIntegrationAuthRepository>();
            _sequenceCaching = _provider.GetRequiredService<ISequenceCaching>();
            _httpClientFactory = _provider.GetRequiredService<IHttpClientFactory>();
            _socketServiceUrl = _provider.GetRequiredService<IConfiguration>().GetValue<string>("SocketServiceUrl");
        }

        public override async void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, ReadOnlyMemory<byte> body)
        {
            _channel.BasicAck(deliveryTag, false);
            try
            {
                var @event = JsonConvert.DeserializeObject<IntegrationSignInEvent>(Encoding.UTF8.GetString(body.ToArray()));
                Logging.Information($"Received an event {typeof(IntegrationSignInEvent).Name} with event id = {@event.EventId} at {@event.Timestamp}");

                var signInBody = JsonConvert.DeserializeObject<SignInBody>(JsonConvert.SerializeObject(@event.Body));
                var requestInfo = await GetRequestInfoASync(signInBody.RequestId);
                if (requestInfo == null)
                {
                    Logging.Error(new DataNotFoundException($"Could not find request info with request id: {signInBody.RequestId}"));
                    _channel.BasicAck(deliveryTag, false);
                    return;
                }

                var ipInfo = await AuthUtility.GetIpInformationAsync(_provider, requestInfo.Ip);
                if (ipInfo == null)
                {
                    Logging.Error(new DataNotFoundException($"Could not find ip information with ip value: {requestInfo.Ip}"));
                    _channel.BasicAck(deliveryTag, false);
                    return;
                }

                var requestValue = new RequestValue
                {
                    Ip = ipInfo.Ip,
                    IpInformation = ipInfo,
                    Browser = requestInfo.Browser,
                    OS = requestInfo.OS,
                    Device = requestInfo.Device,
                    Origin = requestInfo.Origin,
                    UA = requestInfo.UA
                };

                var notification = GetNotification(signInBody.TokenUser, requestValue, @event.Timestamp);
                var noticeTask = _authNoticeService.SignInWarningAsync(signInBody.TokenUser, requestValue, @event.Timestamp);
                var loggingTask = GetSaveLoggingTask(signInBody.TokenUser, requestValue, @event.Timestamp);
                var notificationTask = SaveNotificationAsync(signInBody.TokenUser, requestValue, notification);

                await Task.WhenAll(noticeTask, loggingTask, notificationTask);

                var client = _httpClientFactory.CreateClient();
                var message = new NotificationMessage
                {
                    Type = MessageHubType.SignIn,
                    IsAllClients = false,
                    Description = notification.Description,
                    Keys = new List<string> { $"{notification.TenantId}_{notification.OwnerId}" }
                };
                var httpResponse = await client.PostAsJsonAsync($"{_socketServiceUrl}/send-message", message);
                var response = JsonConvert.DeserializeObject<BaseResponse>(await httpResponse.Content.ReadAsStringAsync());
                if (response.Status != "success")
                {
                    Logging.Error(response.Error?.Message ?? "Unknown error when attempt send socket to client");
                }

            }
            catch (Exception ex)
            {
                Logging.Error(ex, $"{GetType().Name} occurred an error");
                throw;
            }
        }

        private async Task<RequestInformation> GetRequestInfoASync(string requestId)
        {
            using (var dbConnection = new DbConnection("CentralizedRequestsDb"))
            {
                var cmd = $"SELECT * FROM {new RequestInformation().GetTableName()} WHERE RequestId = @RequestId";
                return await dbConnection.QuerySingleOrDefaultAsync<RequestInformation>(cmd, new { RequestId = requestId });
            }
        }

        private Task GetSaveLoggingTask(User user, RequestValue requestValue, DateTime timestamp)
        {
            var history = new SignInHistory
            {
                Id = AuthUtility.GenerateSnowflakeId(),
                SignInTime = timestamp,
                CreatedDate = DateHelper.Now,
                TenantId = user.TenantId,
                Username = user.Username,
                Ip =requestValue.Ip,
                Browser = requestValue.Browser,
                OS = requestValue.OS,
                Device = requestValue.Device,
                UA = requestValue.UA,
                City = requestValue.IpInformation.City,
                Country = requestValue.IpInformation.Country,
                Lat = requestValue.IpInformation.Loc?.Split(",")[0],
                Long = requestValue.IpInformation.Loc?.Split(",")[1],
                Org = requestValue.IpInformation.Org,
                Postal = requestValue.IpInformation.Postal,
                Origin = requestValue.Origin,
                Timezone = requestValue.IpInformation.Timezone,
            };
            return _authRepository.WriteSignInAsync(history, default(CancellationToken));
        }

        private Notification GetNotification(User user, RequestValue requestValue, DateTime timestamp)
        {
            return new Notification
            {
                Id = AuthUtility.GenerateSnowflakeId(),
                Type = NotificationType.SignIn,
                IsUnread = true,
                Description = "Ai đó vừa đăng nhập vào tài khoản này từ địa chỉ IP " + requestValue.Ip,
                Timestamp = timestamp,
                TenantId = user.TenantId,
                OwnerId = user.Id,
                CreatedBy = user.Id,
                CreatedDate = DateHelper.Now
            };
        }

        private async Task SaveNotificationAsync(User user, RequestValue requestValue, Notification notification)
        {
            using (var dbConnection = new DbConnection("CentralizedNotificationsDb"))
            {
                var properties = notification.GetType().GetProperties().Where(p => p.GetIndexParameters().Length == 0 && !p.IsDefined(typeof(IgnoreAttribute), true));
                var columns = string.Join(", ", properties.Select(p => $"`{p.Name}`"));
                var parameters = string.Join(", ", properties.Select(p => $"@{p.Name}"));
                var cmd = @$"INSERT INTO {new Notification().GetTableName()} ( {columns} ) VALUES ( {parameters} );";

                await dbConnection.ExecuteAsync(cmd, notification, autoCommit: true);
            }
        }
    }
}
