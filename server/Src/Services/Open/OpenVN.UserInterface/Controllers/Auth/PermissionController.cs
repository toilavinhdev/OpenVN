using Microsoft.AspNetCore.Mvc;
using SharedKernel.Libraries;
using SharedKernel.Runtime.Exceptions;

namespace OpenVN.Api
{
    [Route("api/v1/opensync/permission")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        [HttpGet("check")]
        public IActionResult Get(string p, string a)
        {
            return Ok(new SimpleDataResult { Data = AuthUtility.ComparePermissionAsString(p, a) });
        }

        [HttpGet("fetp")]
        public IActionResult ConvertToPermission(int exponent)
        {
            if (exponent < 0 || exponent > 100000)
                throw new BadRequestException("The exponent is not valid");
            return Ok(new SimpleDataResult { Data = AuthUtility.FromExponentToPermission(exponent) });
        }
    }
}
