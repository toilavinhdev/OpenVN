using SharedKernel.Libraries;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    [AuthorizationRequest(new ActionExponent[] { ActionExponent.Cloud })]
    public class GetDirectoryPropertiesQuery : BaseQuery<DirectoryPropertyDto>
    {
        public string Id { get; set; }

        public GetDirectoryPropertiesQuery(string id)
        {
            Id = id;
        }
    }
}
