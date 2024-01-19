using Microsoft.AspNetCore.Http;
using SharedKernel.Libraries;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    [AuthorizationRequest(new ActionExponent[] { ActionExponent.Cloud, ActionExponent.Upload })]
    public class UploadCommand : BaseInsertCommand<List<CloudFileDto>>
    {
        public List<IFormFile> Files { get; }

        public string DirectoryId { get;}

        public UploadCommand(List<IFormFile> files, string directoryId)
        {
            Files = files;
            DirectoryId = directoryId;
        }
    }
}
