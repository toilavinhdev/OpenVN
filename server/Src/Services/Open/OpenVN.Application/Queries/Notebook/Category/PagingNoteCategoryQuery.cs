using SharedKernel.Libraries;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    [AuthorizationRequest(new ActionExponent[] { ActionExponent.Notebook })]
    public class PagingNoteCategoryQuery : BaseQuery<PagingResult<NoteCategoryDto>>
    {
        public PagingRequest PagingRequest { get; }

        public PagingNoteCategoryQuery(PagingRequest pagingRequest)
        {
            PagingRequest = pagingRequest;
        }
    }
}
