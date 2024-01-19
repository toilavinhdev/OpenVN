using SharedKernel.Libraries;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    [AuthorizationRequest(new ActionExponent[] { ActionExponent.Notebook })]
    public class DeleteNoteCategoryCommand : BaseDeleteCommand
    {
        public List<string> Ids { get; }

        public DeleteNoteCategoryCommand(List<string> ids)
        {
            Ids = ids;
        }
    }
}
