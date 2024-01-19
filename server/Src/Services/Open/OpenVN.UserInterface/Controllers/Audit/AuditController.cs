using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Auth;

namespace OpenVN.UserInterface.Controllers
{
    [Authorize]
    [Route("api/v1/opensync/[controller]")]
    [ApiController]
    public class AuditController : ControllerBase
    {
        protected readonly IMediator _mediator;

        public AuditController(IMediator mediator, IToken token)
        {
            _mediator = mediator;
        }

        [HttpGet("paging")]
        public async Task<IActionResult> Get(int page, int size, CancellationToken cancellationToken = default)
        {
            var request = new PagingRequest(page, size);
            var query = new GetAuditPagingQuery(request);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(new ServiceResult { Data = result.Data, Total = result.Count });
        }
    }
}
