using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenVN.Api;
using SharedKernel.Auth;
using SharedKernel.Domain;

namespace OpenVN.UserInterface.Controllers
{
    public class TenantController : BaseController<Tenant>
    {
        public TenantController(IMediator mediator, IToken token) : base(mediator, token)
        {
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetAllTenantQuery(), cancellationToken);
            return Ok(new ServiceResult { Data = result, Total = result.Count });
        }
    }
}
