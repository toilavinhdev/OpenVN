using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Auth;

namespace OpenVN.UserInterface
{
    [Authorize]
    [Route("api/v1/opensync/transfer-cloud")]
    [ApiController]
    public class TransferCloudController : ControllerBase
    {
        protected readonly IMediator _mediator;
        protected readonly IToken _token;
        protected readonly ILogger _logger;

        public TransferCloudController(IMediator mediator, IToken token, ILogger logger)
        {
            _mediator = mediator;
            _token = token;
            _logger = logger;
        }

        [HttpPost("move")]
        public async Task<IActionResult> Move(MoveDto dto, CancellationToken cancellationToken = default)
        {
            var command = new MoveCloudObjectCommand(dto);
            await _mediator.Send(command, cancellationToken);
            return Ok(new BaseResponse());
        }
    }
}
