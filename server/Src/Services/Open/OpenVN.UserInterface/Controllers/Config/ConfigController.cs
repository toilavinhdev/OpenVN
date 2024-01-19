using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Auth;
using SharedKernel.Domain;

namespace OpenVN.Api
{
    public class ConfigController : BaseController<UserConfig>
    {
        public ConfigController(IMediator mediator, IToken token) : base(mediator, token)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken = default)
        {
            return Ok(new SimpleDataResult { Data = await _mediator.Send(new GetConfigQuery(), cancellationToken) });
        }

        [HttpPut]
        public async Task<IActionResult> Put(ConfigValue configValue, CancellationToken cancellationToken = default)
        {
            var command = new UpdateConfigCommand(configValue);
            return Ok(new SimpleDataResult { Data = await _mediator.Send(command, cancellationToken) });
        }
    }
}
