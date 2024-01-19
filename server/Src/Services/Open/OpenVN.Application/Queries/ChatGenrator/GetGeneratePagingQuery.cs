using SharedKernel.Libraries;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    [AuthorizationRequest(new ActionExponent[] { ActionExponent.ChatGenerator })]
    public class GetGeneratePagingQuery : BaseQuery<PagingResult<ChatGeneratorDto>>
    {
        public PagingRequest Request { get; }

        public GetGeneratePagingQuery(PagingRequest request)
        {
            Request = request;
        }
    }
}
