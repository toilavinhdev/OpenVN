using Newtonsoft.Json;
using OpenVN.Audit.Entities;
using OpenVN.Audit.Events;
using OpenVN.Audit.Processes;
using RabbitMQ.Client;
using SharedKernel.Domain;
using SharedKernel.Log;
using System.Text;

namespace OpenVN.Audit.Consumers
{
    public class AuditConsumer : DefaultBasicConsumer
    {
        private readonly IModel _channel;

        public AuditConsumer(IModel channel)
        {
            _channel = channel;
        }

        public override void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, ReadOnlyMemory<byte> body)
        {
            var bodyStr = Encoding.UTF8.GetString(body.ToArray());
            var @event = JsonConvert.DeserializeObject<IntegrationAuditEvent<BaseEntity>>(bodyStr);
            Logging.Information($"Received an audit event with event id = {@event.EventId}");

            if (@event != null)
            {
                switch (@event.TableName)
                {
                    case nameof(Directory):
                        new DirectoryProcess().HandleAsync(bodyStr).GetAwaiter().GetResult();
                        break;
                    case nameof(CloudFile):
                        new FileProcess().HandleAsync(bodyStr).GetAwaiter().GetResult();
                        break;
                    case nameof(Note):
                        new BaseProcess<Note>(new Models.AuditConfigModel("ghi chú")).HandleAsync(bodyStr).GetAwaiter().GetResult();
                        break;
                    case nameof(Avatar):
                        new AvatarProcess().HandleAsync(bodyStr).GetAwaiter().GetResult();
                        break;
                    case nameof(ChatGenerator):
                        new ChatGeneratorProcess().HandleAsync(bodyStr).GetAwaiter().GetResult();
                        break;
                    case "AppMaster":
                        new AppProcess().HandleAsync(bodyStr).GetAwaiter().GetResult();
                        break;
                    case "SignIn":
                        new SignInProcess().HandleAsync(bodyStr).GetAwaiter().GetResult();
                        break;
                    case "SignOut":
                        new SignOutProcess().HandleAsync(bodyStr).GetAwaiter().GetResult();
                        break;
                    default:
                        Logging.Warning("Not found any handler with name = " + @event.TableName);
                        break;
                }
            }

            _channel.BasicAck(deliveryTag, false);
        }
    }
}
