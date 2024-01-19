using SharedKernel.Libraries;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    [AuthorizationRequest(new ActionExponent[] { ActionExponent.SA })]
    public class SetPermissionCommand : BaseUpdateCommand
    {
        public SetPermissionDto SetPermissionDto { get; }

        public SetPermissionCommand(SetPermissionDto setPermissionDto)
        {
            SetPermissionDto = setPermissionDto;
        }
    }
}
