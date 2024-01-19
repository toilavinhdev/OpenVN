using SharedKernel.Libraries;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    [AuthorizationRequest(new ActionExponent[] { ActionExponent.Cloud })]
    public class GetAllDirectoryQuery : BaseQuery<List<DirectoryDto>>
    {
        public string DirectoryId { get;}

        public GetAllDirectoryQuery(string directoryId)
        {
            DirectoryId = directoryId;
        }
    }
}
