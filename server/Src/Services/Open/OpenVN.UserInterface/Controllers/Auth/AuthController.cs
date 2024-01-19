using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OpenVN.Api
{
    [Authorize]
    [Route("api/v1/opensync/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok(new SimpleDataResult { Data = "pong" });
        }

        #region SignIn + SignOut
        [AllowAnonymous]
        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn(SignInRequestDto request, CancellationToken cancellationToken = default)
        {
            var command = new SignInCommand(request.UserName, request.Password);
            return Ok(await _mediator.Send(command, cancellationToken));
        }

        [HttpGet("sign-out")]
        public async Task<IActionResult> SignOut(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new SignOutCommand(), cancellationToken);
            return Ok(new BaseResponse());
        }

        //[HttpGet("revoke")]
        //public async Task<IActionResult> SignOutAllDevice(string userId, CancellationToken cancellationToken = default)
        //{
        //    throw new NotImplementedException();
        //    //var result = await _authService.SignOutAllDeviceAsync(userId, cancellationToken);
        //    //return Ok(new BaseResponse { Success = result });
        //}

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequestDto request, CancellationToken cancellationToken = default)
        {
            var command = new RefreshTokenCommand(request.UserId, request.RefreshToken);
            return Ok(await _mediator.Send(command, cancellationToken));
        }
        #endregion

        #region SignUp
        [AllowAnonymous]
        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp(CreateUserDto request, CancellationToken cancellationToken = default)
        {
            var uid = await _mediator.Send(new SignUpCommand(request), cancellationToken);
            return Ok(new SimpleDataResult { Data = uid });
        }

        #endregion

        [HttpGet("history-paging")]
        public async Task<IActionResult> SignInHistoryPaging(int page, int size, CancellationToken cancellationToken = default)
        {
            var query = new SignInHistoryPagingQuery(new PagingRequest(page, size));
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(new ServiceResult { Data = result.Data, Total = result.Count });
        }

        [HttpGet("request-information"), AllowAnonymous]
        public async Task<IActionResult> RequestInformation(CancellationToken cancellationToken = default)
        {
            var query = new GetRequestInformationQuery();
            return Ok(new SimpleDataResult { Data = await _mediator.Send(query, cancellationToken) });
        }
    }
}
