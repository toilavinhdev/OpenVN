using SharedKernel.Libraries;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    [AuthorizationRequest(new ActionExponent[] { ActionExponent.Cloud })]
    public class GetCapacityConfigurationQuery : BaseQuery<CapacityConfigurationDto>
    {
    }
}
