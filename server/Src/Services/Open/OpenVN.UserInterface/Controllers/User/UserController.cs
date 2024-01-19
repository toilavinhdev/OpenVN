using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Auth;
using SharedKernel.Domain;

namespace OpenVN.Api
{
    public class UserController : BaseController<User>
    {
        public UserController(IMediator mediator, IToken token) : base(mediator, token)
        {
        }

        [HttpGet("user-information")]
        public async Task<IActionResult> GetUserInformation(CancellationToken cancellationToken = default)
        {
            var query = new GetUserInformationQuery();
            return Ok(new SimpleDataResult { Data = await _mediator.Send(query, cancellationToken) });
        }

        [HttpGet("avatar")]
        public async Task<IActionResult> GetAvatar(CancellationToken cancellationToken = default)
        {
            var query = new GetAvatarQuery();
            return Ok(new SimpleDataResult { Data = await _mediator.Send(query, cancellationToken) });
        }

        [HttpPost("set-avatar")]
        public async Task<IActionResult> SetAvatar(IFormFile avatar, CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new SetAvatarCommand(avatar), cancellationToken);
            return Ok(new BaseResponse());
        }

        [HttpDelete("remove-avatar")]
        public async Task<IActionResult> RemoveAvatar(CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new RemoveAvatarCommand(), cancellationToken);
            return Ok(new BaseResponse());
        }
    }
}
