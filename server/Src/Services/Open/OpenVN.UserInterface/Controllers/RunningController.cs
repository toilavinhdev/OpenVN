using Microsoft.AspNetCore.Mvc;
using SharedKernel.Log;

namespace OpenVN.UserInterface.Controllers
{
    [Route("api/keeper")]
    [ApiController]
    public class RunningController : ControllerBase
    {
        [HttpGet("ping")]
        public IActionResult Get()
        {
            Logging.LogCustom("ping", "pong");
            return Ok("pong");
        }
    }
}
