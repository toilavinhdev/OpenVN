using SharedKernel.Libraries;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    [AuthorizationRequest(new ActionExponent[] { ActionExponent.Cloud })]
    public class GetDirectoryPathQuery : BaseQuery<PathDto>
    {
        public string DirectoryId { get; }
        public GetDirectoryPathQuery(string directoryId)
        {
            DirectoryId = directoryId;
        }
    }
}
