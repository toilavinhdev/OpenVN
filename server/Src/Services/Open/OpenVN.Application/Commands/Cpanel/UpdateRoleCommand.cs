using SharedKernel.Libraries;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    [AuthorizationRequest(new ActionExponent[] { ActionExponent.SA })]
    public class UpdateRoleCommand : BaseUpdateCommand
    {
        public UpdateRoleDto Dto { get; }

        public UpdateRoleCommand(UpdateRoleDto dto)
        {
            Dto = dto;
        }
    }
}
