using SharedKernel.Libraries;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    [AuthorizationRequest(new ActionExponent[] { ActionExponent.Notebook })]
    public class UpdateNoteCategoryCommand : BaseUpdateCommand
    {
        public NoteCategoryDto NoteCategoryDto { get; }
        public UpdateNoteCategoryCommand(NoteCategoryDto noteCategoryDto)
        {
            NoteCategoryDto = noteCategoryDto;
        }
    }
}
