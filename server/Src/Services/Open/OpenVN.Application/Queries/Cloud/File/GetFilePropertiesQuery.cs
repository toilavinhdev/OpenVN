using SharedKernel.Libraries;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    [AuthorizationRequest(new ActionExponent[] { ActionExponent.Cloud })]
    public class GetFilePropertiesQuery : BaseQuery<FilePropertyDto>
    {
        public string FileId { get; }

        public GetFilePropertiesQuery(string fileId)
        {
            FileId = fileId;
        }
    }
}
