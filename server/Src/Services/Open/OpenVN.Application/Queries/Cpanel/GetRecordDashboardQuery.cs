using SharedKernel.Libraries;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application.Queries.Cpanel
{
    [AuthorizationRequest(new ActionExponent[] { ActionExponent.SA })]
    public class GetRecordDashboardQuery : BaseQuery<List<RecordDashboardDto>>
    {
    }
}
