using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenVN.Application.Queries.Cpanel;
using static SharedKernel.Application.Enum;

namespace OpenVN.Api
{
    [Authorize]
    [Route("api/v1/opensync/cpanel")]
    [ApiController]
    public class CpanelController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IAuthService _authService;

        public CpanelController(IMediator mediator, IAuthService authService)
        {
            _mediator = mediator;
            _authService = authService;
        }

        [HttpGet("role/roles")]
        public async Task<IActionResult> GetRoles(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetRolesQuery(), cancellationToken);
            return Ok(new ServiceResult { Data = result, Total = result.Count });
        }

        [HttpPut("role/update-role")]
        public async Task<IActionResult> UpdateRole(UpdateRoleDto dto, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new UpdateRoleCommand(dto), cancellationToken);
            return Ok(new BaseResponse());
        }

        [HttpGet("dashboard/total-records")]
        public async Task<IActionResult> GetRecordDashboard(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetRecordDashboardQuery(), cancellationToken);
            return Ok(new SimpleDataResult { Data = result });
        }

        [HttpGet("user/users")]
        public async Task<IActionResult> GetUsers(int page, int size, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetUserPagingQuery(new PagingRequest(page, size)), cancellationToken);
            return Ok(new ServiceResult { Data = result.Data, Total = result.Count });
        }

        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser(CreateUserDto request, CancellationToken cancellationToken = default)
        {
            var uid = await _mediator.Send(new CreateUserCommand(request), cancellationToken);
            return Ok(new SimpleDataResult { Data = uid });
        }

        [HttpPost("set-permission")]
        public async Task<IActionResult> SetPermission(SetPermissionDto request, CancellationToken cancellationToken = default)
        {
            var uid = await _mediator.Send(new SetPermissionCommand(request), cancellationToken);
            return Ok(new BaseResponse());
        }
    }
}
