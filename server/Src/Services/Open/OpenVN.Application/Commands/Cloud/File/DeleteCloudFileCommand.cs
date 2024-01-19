using SharedKernel.Libraries;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    [AuthorizationRequest(new ActionExponent[] { ActionExponent.Cloud })]
    public class DeleteCloudFileCommand : BaseDeleteCommand
    {
        public string DirectoryId { get; }

        public List<string> Ids { get; }

        public DeleteCloudFileCommand(string directoryId, List<string> ids)
        {
            DirectoryId = directoryId;
            Ids = ids;
        }
    }
}
