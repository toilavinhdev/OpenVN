using MediatR;
using Microsoft.AspNetCore.Mvc;
using OpenVN.Api;
using OpenVN.Domain;
using SharedKernel.Auth;

namespace OpenVN.UserInterface.Controllers
{
    public class ChatGeneratorController : BaseController<ChatGenerator>
    {
        public ChatGeneratorController(IMediator mediator, IToken token) : base(mediator, token)
        {
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetGenerateByIdQuery(id), cancellationToken);
            return Ok(new SimpleDataResult { Data = result });
        }

        [HttpGet("paging")]
        public async Task<IActionResult> Get(int page, int size, CancellationToken cancellationToken = default)
        {
            var query = new GetGeneratePagingQuery(new PagingRequest(page, size));
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(new ServiceResult { Data = result.Data, Total = result.Count });
        }

        [DisableRequestSizeLimit]
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file, CancellationToken cancellationToken = default)
        {
            var command = new AnalyseTextCommand(file);
            return Ok(new SimpleDataResult { Data = await _mediator.Send(command, cancellationToken) });
        }
    }
}
