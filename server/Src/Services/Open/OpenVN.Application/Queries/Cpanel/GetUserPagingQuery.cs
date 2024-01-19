using SharedKernel.Libraries;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    [AuthorizationRequest(new ActionExponent[] { ActionExponent.SA })]
    public class GetUserPagingQuery : BaseQuery<PagingResult<UserDto>>
    {
        public PagingRequest Request { get; }

        public GetUserPagingQuery(PagingRequest request)
        {
            Request = request;
        }
    }
}
