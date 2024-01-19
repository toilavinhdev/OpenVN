using SharedKernel.Libraries;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    [AuthorizationRequest(new ActionExponent[] { ActionExponent.Cloud })]
    public class SetPasswordCommand : BaseInsertCommand
    {
        public SetPasswordDto SetPasswordDto { get; }

        public SetPasswordCommand(SetPasswordDto setPasswordDto)
        {
            SetPasswordDto = setPasswordDto;
        }
    }
}
