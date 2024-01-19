using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Auth;

namespace OpenVN.UserInterface.Controllers
{
    [Authorize]
    [Route("api/v1/opensync/[controller]")]
    [ApiController]
    public class AuthCloudController : ControllerBase
    {
        protected readonly IMediator _mediator;
        protected readonly IToken _token;
        protected readonly ILogger _logger;

        public AuthCloudController(IMediator mediator, IToken token, ILogger logger)
        {
            _mediator = mediator;
            _token = token;
            _logger = logger;
        }

        [HttpPost("gen-code")]
        public async Task<IActionResult> CreateSecretKey(GenDirectoryCodeDto dto, CancellationToken cancellationToken = default)
        {
            var command = new CreateCodeCommand(dto.DirectoryId, dto.Password);
            var code = await _mediator.Send(command, cancellationToken);
            return Ok(new SimpleDataResult { Data = code });
        }

        [HttpPost("extend-code")]
        public async Task<IActionResult> ExtendCode(ExtendCodeDto dto, CancellationToken cancellationToken = default)
        {
            var command = new ExtendCodeCommand(dto.DirectoryId, dto.Code);
            await _mediator.Send(command, cancellationToken);
            return Ok(new BaseResponse());
        }

        [HttpPost("set-password")]
        public async Task<IActionResult> SetPassword(SetPasswordDto dto, CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new SetPasswordCommand(dto), cancellationToken);
            return Ok(new BaseResponse());
        }

        [HttpPut("lock/{directoryId}")]
        public async Task<IActionResult> Lock(string directoryId, CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new ChangeDirectoryLockCommand(directoryId, "lock", ""), cancellationToken);
            return Ok(new BaseResponse());
        }

        [HttpPut("unlock")]
        public async Task<IActionResult> Unlock(UnlockDirectoryDto dto, CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new ChangeDirectoryLockCommand(dto.DirectoryId, "unlock", dto.Password), cancellationToken);
            return Ok(new BaseResponse());
        }
    }
}
