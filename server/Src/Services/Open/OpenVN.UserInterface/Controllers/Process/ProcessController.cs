using MediatR;
using Microsoft.AspNetCore.Mvc;
using OpenVN.Domain;
using SharedKernel.Auth;

namespace OpenVN.Api
{
    public class ProcessController : BaseController<Process>
    {
        public ProcessController(IMediator mediator, IToken token) : base(mediator, token)
        {
        }

        [HttpPost]
        public async Task<IActionResult> Post(ProcessDto process, CancellationToken cancellationToken = default)
        {
            var command = new CreateProcessCommand(process);
            return Ok(new SimpleDataResult { Data = await _mediator.Send(command, cancellationToken)});
        }
    }
}
