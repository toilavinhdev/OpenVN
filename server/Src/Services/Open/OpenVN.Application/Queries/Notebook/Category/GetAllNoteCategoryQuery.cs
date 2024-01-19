using SharedKernel.Libraries;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    [AuthorizationRequest(new ActionExponent[] { ActionExponent.Notebook })]
    public class GetAllNoteCategoryQuery : BaseQuery<List<NoteCategoryDto>>
    {
    }
}
