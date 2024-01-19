using MediatR;
using Microsoft.AspNetCore.Mvc;
using OpenVN.Api;
using OpenVN.Domain;
using SharedKernel.Auth;

namespace OpenVN.UserInterface.Controllers
{
    public class TestDeadlockController : BaseController<TestDeadlock>
    {
        public TestDeadlockController(IMediator mediator, IToken token) : base(mediator, token)
        {
        }

        [HttpGet("run")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken = default)
        {
            return Ok(await _mediator.Send(new TestDeadlockQuery(), cancellationToken));
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add(CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new TestDeadlockCommand(), cancellationToken);
            return Ok();
        }
    }
}
