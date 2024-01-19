using SharedKernel.Libraries;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    [AuthorizationRequest(new ActionExponent[] { ActionExponent.Notebook })]
    public class GetNotesQuery : BaseQuery<List<NoteWithoutContentDto>>
    {
        public string Query { get; }
        public GetNotesQuery(string query)
        {
            Query = query;
        }
    }
}
