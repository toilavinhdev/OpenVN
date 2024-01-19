using SharedKernel.Libraries;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    [AuthorizationRequest(new ActionExponent[] { ActionExponent.Cloud })]
    public class GetFilesInDirQuery : BaseQuery<List<CloudFileDto>>
    {
        public string DirectoryId { get; }
        public string ConnectionId { get; }

        public GetFilesInDirQuery(string directoryId, string connectionId)
        {
            DirectoryId = directoryId;
            ConnectionId = connectionId;
        }
    }
}
