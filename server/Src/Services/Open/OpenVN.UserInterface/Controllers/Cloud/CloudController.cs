using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;
using OpenVN.Api;
using OpenVN.Domain;
using SharedKernel.Auth;
using SharedKernel.Properties;
using SharedKernel.Runtime.Exceptions;
using SharedKernel.SignalR;

namespace OpenVN.UserInterface.Controllers
{
    //[RequiredSecretKey(CloudConstant.SECRET_KEY_NAME)]
    public class CloudController : BaseController<CloudFile>
    {
        private readonly IStringLocalizer<Resources> _localizer;

        public CloudController(
            IMediator mediator,
            IToken token,
            IStringLocalizer<Resources> localizer
        ) : base(mediator, token)
        {
            _localizer = localizer;
        }

        [DisableRequestSizeLimit]
        [HttpPost("upload/{directoryId}")]
        public async Task<IActionResult> Upload(string directoryId, List<IFormFile> files, CancellationToken cancellationToken = default)
        {
            var command = new UploadCommand(files, directoryId);
            //Merge();
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(new SimpleDataResult { Data = result });
        }

        [HttpGet("files-in-dir/{directoryId}")]
        public async Task<IActionResult> FilesInDir(string directoryId, string connectionId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(connectionId))
            {
                throw new BadRequestException(_localizer["bad_data"]);
            }

            var query = new GetFilesInDirQuery(directoryId, connectionId);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(new ServiceResult { Data = result, Total = result.Count });
        }

        [HttpDelete("{directoryId}")]
        public async Task<IActionResult> Delete(string directoryId, List<string> ids, CancellationToken cancellationToken = default)
        {
            var command = new DeleteCloudFileCommand(directoryId, ids);
            await _mediator.Send(command, cancellationToken);
            return Ok(new BaseResponse());
        }

        [HttpGet("capacity-configuration")]
        public async Task<IActionResult> CapacityConfig(CancellationToken cancellationToken = default)
        {
            var query = new GetCapacityConfigurationQuery();
            return Ok(new SimpleDataResult { Data = await _mediator.Send(query, cancellationToken) });
        }

        [HttpGet("count-information")]
        public async Task<IActionResult> GetTotalRecordCount(CancellationToken cancellationToken = default)
        {
            var query = new GetCountInformationQuery();
            return Ok(new SimpleDataResult { Data = await _mediator.Send(query, cancellationToken) });
        }

        [HttpGet("properties/{id}")]
        public async Task<IActionResult> GetProperties(string id, CancellationToken cancellationToken = default)
        {
            var query = new GetFilePropertiesQuery(id);
            return Ok(new SimpleDataResult { Data = await _mediator.Send(query, cancellationToken) });
        }
    }
}
