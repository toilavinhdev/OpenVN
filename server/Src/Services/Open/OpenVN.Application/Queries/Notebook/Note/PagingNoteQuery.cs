using SharedKernel.Libraries;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    [AuthorizationRequest(new ActionExponent[] { ActionExponent.Notebook })]
    public class PagingNoteQuery : BaseQuery<PagingResult<NoteWithoutContentDto>>
    {
        public PagingRequest PagingRequest { get; }

        public PagingNoteQuery(PagingRequest pagingRequest)
        {
            PagingRequest = pagingRequest;
        }
    }
}
