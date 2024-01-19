using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenVN.Master.Application;
using OpenVN.Master.Application.Commands;
using SharedKernel.Application;

namespace OpenVN.Master.Api.Controllers
{
    [ApiController]
    [Route("api/v1/master/[controller]")]
    [Authorize]
    public class AppController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AppController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> GetApps(CancellationToken cancellationToken = default)
        {
            var query = new GetAppQuery();
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(new ServiceResult { Data = result, Total = result.Count });
        }

        [HttpPut("update-favourite")]
        public async Task<IActionResult> UpdateFavourite(string appId, bool isFavourite, CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new UpdateFavouriteCommand(appId, isFavourite), cancellationToken);
            return Ok(new BaseResponse());
        }
    }
}