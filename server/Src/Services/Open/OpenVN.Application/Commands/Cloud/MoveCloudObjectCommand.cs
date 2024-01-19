using SharedKernel.Libraries;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    [AuthorizationRequest(new ActionExponent[] { ActionExponent.Cloud })]
    public class MoveCloudObjectCommand : BaseUpdateCommand
    {
        public MoveDto MoveDto { get; }
        public MoveCloudObjectCommand(MoveDto moveDto)
        {
            MoveDto = moveDto;
        }
    }
}
