using MediatR;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Auth;
using SharedKernel.Domain;
using SharedKernel.Providers;

namespace OpenVN.Api.Controllers.Info
{
    public class InformationController : BaseController<BaseEntity>
    {
        private readonly IApplicationConfiguration _appSettingConfiguration;

        public InformationController(
            IMediator mediator, 
            IToken token, 
            IApplicationConfiguration appSettingConfiguration
        ) : base(mediator, token)
        {
            _appSettingConfiguration = appSettingConfiguration;
        }

        [HttpGet]
        public IActionResult GetInformation()
        {
            return Ok(new SimpleDataResult {  Data = _appSettingConfiguration.GetConfiguration<Information>()});
        }
    }
}
