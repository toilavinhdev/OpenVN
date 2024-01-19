using SharedKernel.Libraries;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    [AuthorizationRequest(new ActionExponent[] { ActionExponent.Notebook })]
    public class UpdateNoteCommand : BaseUpdateCommand
    {
        public ChangeNoteDto ChangeNoteDto { get;}

        public int Type { get; }

        public UpdateNoteCommand(ChangeNoteDto changeNoteDto, int type)
        {
            ChangeNoteDto = changeNoteDto;
            Type = type;
        }
    }
}
