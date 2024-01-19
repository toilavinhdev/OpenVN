using SharedKernel.Libraries;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    [AuthorizationRequest(new ActionExponent[] { ActionExponent.Notebook })]
    public class CreateNoteCommand : BaseInsertCommand<string>
    {
        public NoteDto NoteDto { get; set; }

        public CreateNoteCommand(NoteDto noteDto)
        {
            NoteDto = noteDto;
        }
    }
}
