using SharedKernel.Libraries;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    [AuthorizationRequest(new ActionExponent[] { ActionExponent.Cloud })]
    public class AddDirectoryCommand : BaseInsertCommand<DirectoryDto>
    {
        public DirectoryDto DirectoryDto { get;}

        public AddDirectoryCommand(DirectoryDto directoryDto)
        {
            DirectoryDto = directoryDto;
        }
    }
}
