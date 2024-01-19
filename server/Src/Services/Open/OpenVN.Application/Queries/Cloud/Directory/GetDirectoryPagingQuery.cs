using SharedKernel.Libraries;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    [AuthorizationRequest(new ActionExponent[] { ActionExponent.Cloud })]
    public class GetDirectoryPagingQuery : BaseQuery<PagingResult<DirectoryDto>>
    {
        public PagingRequest PagingRequest { get; }
        public string DirectoryId { get; }

        public GetDirectoryPagingQuery(string directoryId, PagingRequest pagingRequest)
        {
            DirectoryId = directoryId;
            PagingRequest = pagingRequest;
        }
    }
}
