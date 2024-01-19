using SharedKernel.Libraries;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    [AuthorizationRequest(new ActionExponent[] { ActionExponent.Notebook })]
    public class CreateNoteCategoryCommand : BaseInsertCommand<string>
    {
        public NoteCategoryDto NoteCategoryDto { get; set; }

        public CreateNoteCategoryCommand(NoteCategoryDto noteCategoryDto)
        {
            NoteCategoryDto = noteCategoryDto;
        }
    }
}
