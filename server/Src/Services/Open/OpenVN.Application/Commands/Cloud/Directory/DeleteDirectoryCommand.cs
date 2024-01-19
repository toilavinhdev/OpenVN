using SharedKernel.Libraries;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    [AuthorizationRequest(new ActionExponent[] { ActionExponent.Cloud })]
    public class DeleteDirectoryCommand : BaseDeleteCommand
    {
        public List<string> Ids { get; }

        public DeleteDirectoryCommand(List<string> ids)
        {
            Ids = ids;
        }
    }
}
