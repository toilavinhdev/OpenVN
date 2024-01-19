using SharedKernel.Libraries;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    [AuthorizationRequest(new ActionExponent[] { ActionExponent.Notebook })]
    public class DeleteNoteCommand : BaseDeleteCommand
    {
        public List<string> Ids { get; }

        public DeleteNoteCommand(List<string> ids)
        {
            Ids = ids;
        }
    }
}
