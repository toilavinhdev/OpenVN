using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenVN.Api;
using OpenVN.Domain;
using SharedKernel.Auth;

namespace OpenVN.UserInterface.Controllers
{
    [AllowAnonymous]
    public class FeedbackController : BaseController<Feedback>
    {
        public FeedbackController(IMediator mediator, IToken token) : base(mediator, token)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetAllFeedbackQuery(), cancellationToken);
            return Ok(new ServiceResult { Data = result, Total = result.Count });
        }

        [HttpGet("paging")]
        public async Task<IActionResult> Get(int page, int size, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new PagingFeedbackQuery(new PagingRequest(page, size)), cancellationToken);
            return Ok(new ServiceResult { Data = result.Data, Total = result.Count });
        }

        [HttpGet("replies/{id}")]
        public async Task<IActionResult> Get(string id, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetRepliesFeedbackQuery(id), cancellationToken);
            return Ok(new ServiceResult { Data = result, Total = result.Count });
        }

        [HttpPost]
        public async Task<IActionResult> Post(FeedbackDto dto, CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new AddFeedbackCommand(dto), cancellationToken);
            return Ok(new BaseResponse());
        }
    }
}
