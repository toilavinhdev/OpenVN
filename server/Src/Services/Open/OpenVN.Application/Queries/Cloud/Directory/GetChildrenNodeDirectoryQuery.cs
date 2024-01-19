using SharedKernel.Libraries;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    [AuthorizationRequest(new ActionExponent[] { ActionExponent.Cloud })]
    public class GetChildrenNodeDirectoryQuery : BaseQuery<List<string>>
    {
        public string RootId { get; }

        public GetChildrenNodeDirectoryQuery(string rootId)
        {
            RootId = rootId;
        }
    }
}
