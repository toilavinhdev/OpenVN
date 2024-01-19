using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenVN.Api;
using OpenVN.Application;
using SharedKernel.Auth;
using SharedKernel.Domain;
using SharedKernel.Log;
using SharedKernel.SignalR;

namespace OpenVN.UserInterface.Controllers.Nofitication
{
    public class NotificationController : BaseController<Notification>
    {
        public NotificationController(IMediator mediator, IToken token) : base(mediator, token)
        {
        }

        [AllowAnonymous]
        [HttpGet("connections")]
        public async Task<IActionResult> GetConnections(CancellationToken cancellationToken = default)
        {
            await Task.Yield();
            return Ok(new ServiceResult { Data = MessageHub.Connections, Total = MessageHub.Connections.Count });
        }

        [AllowAnonymous]
        [HttpGet("key-value")]
        public async Task<IActionResult> GetKeyValues(CancellationToken cancellationToken = default)
        {
            await Task.Yield();
            return Ok(new ServiceResult { Data = MessageHub.KeyValueConnections, Total = MessageHub.KeyValueConnections.Count });
        }

        [AllowAnonymous]
        [HttpGet("online-users")]
        public async Task<IActionResult> GetOnlineUsers(CancellationToken cancellationToken = default)
        {
            await Task.Yield();
            return Ok(new SimpleDataResult { Data = MessageHub.KeyValueConnections.Count() });
        }

        [AllowAnonymous]
        [HttpGet("online-detail")]
        public async Task<IActionResult> GetOnlineDetail(CancellationToken cancellationToken = default)
        {
            await Task.Yield();
            var identityCount = MessageHub.KeyValueConnections.Count(x => !x.Key.Equals(x.Value));

            return Ok(new SimpleDataResult { Data = new { Identity = identityCount, Anonymous = MessageHub.KeyValueConnections.Count()  - identityCount } });
        }

        [AllowAnonymous]
        [HttpPost("send-message")]
        public async Task<IActionResult> SendMessage(NotificationMessageDto message, CancellationToken cancellationToken = default)
        {
            Logging.LogCustom("socket", "Received a request send socket...");
            await _mediator.Send(new SendMessageCommand(message), cancellationToken);
            return Ok(new BaseResponse());
        }

        [HttpGet("number-of-unread-notification")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken = default)
        {
            var query = new GetNumberOfUnreadNotificationQuery();
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(new SimpleDataResult { Data = result });
        }

        [HttpGet("paging")]
        public async Task<IActionResult> Get(int page, int size, string type, CancellationToken cancellationToken = default)
        {
            var request = new PagingRequest(page, size);
            if (type == "unread")
            {
                request.Filter = new Filter
                {
                    Fields = new List<Field>
                    {
                        new Field
                        {
                            FieldName = "IsUnread",
                            Value = "1"
                        }
                    },
                    Formula = "{0}"
                };
            }
            request.Sorts = new List<SortModel>
            {
                new SortModel
                {
                    FieldName = "Timestamp",
                    SortAscending = false
                }
            };

            var query = new GetNotificationPagingQuery(request);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(new ServiceResult { Data = result.Data, Total = result.Count });
        }

        [HttpPut("mark-as-unread/{id}")]
        public async Task<IActionResult> MarkAsUnread(string id, CancellationToken cancellationToken = default)
        {
            var command = new MarkAsReadOrUnreadCommand(id, false);
            await _mediator.Send(command, cancellationToken);

            return Ok(new BaseResponse());
        }

        [HttpPut("mark-as-read/{id}")]
        public async Task<IActionResult> MarkAsRead(string id, CancellationToken cancellationToken = default)
        {
            var command = new MarkAsReadOrUnreadCommand(id);
            await _mediator.Send(command, cancellationToken);

            return Ok(new BaseResponse());
        }

        [HttpPut("mark-all-as-read")]
        public async Task<IActionResult> MarkAllAsRead(CancellationToken cancellationToken = default)
        {
            var command = new MarkAllAsReadCommand();
            await _mediator.Send(command, cancellationToken);

            return Ok(new BaseResponse());
        }
    }
}
