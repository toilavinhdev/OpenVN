using SharedKernel.Libraries;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    [AuthorizationRequest(new ActionExponent[] { ActionExponent.Notebook, ActionExponent.AllowAnonymous })]
    public class GetNoteByIdQuery : BaseAllowAnonymousQuery<NoteDto>
    {
        public string Id { get;}

        public GetNoteByIdQuery(string id)
        {
            Id = id;
        }
    }
}
