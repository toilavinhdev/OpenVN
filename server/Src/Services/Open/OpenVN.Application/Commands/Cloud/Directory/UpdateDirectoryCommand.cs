using SharedKernel.Libraries;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    [AuthorizationRequest(new ActionExponent[] { ActionExponent.Cloud })]
    public class UpdateDirectoryCommand : BaseUpdateCommand
    {
        public DirectoryDto DirectoryDto { get; }

        public UpdateDirectoryCommand(DirectoryDto directoryDto)
        {
            DirectoryDto = directoryDto;
        }
    }
}
