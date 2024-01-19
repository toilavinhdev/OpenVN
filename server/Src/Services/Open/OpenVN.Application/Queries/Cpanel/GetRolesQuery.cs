using OpenVN.Application.Dto.Cpanel;
using SharedKernel.Libraries;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    [AuthorizationRequest(new ActionExponent[] { ActionExponent.SA })]
    public class GetRolesQuery : BaseQuery<List<RoleDto>>
    {
    }
}
