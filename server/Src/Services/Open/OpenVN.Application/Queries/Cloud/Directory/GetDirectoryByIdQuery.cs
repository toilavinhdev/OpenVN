using SharedKernel.Libraries;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    [AuthorizationRequest(new ActionExponent[] { ActionExponent.Cloud })]
    public class GetDirectoryByIdQuery : BaseQuery<DirectoryDto>
    {
        public string Id { get; set; }

        public GetDirectoryByIdQuery(string id)
        {
            Id = id;
        }
    }
}
