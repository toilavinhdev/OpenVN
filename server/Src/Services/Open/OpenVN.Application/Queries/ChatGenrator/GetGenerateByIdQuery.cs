using SharedKernel.Libraries;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    [AuthorizationRequest(new ActionExponent[] { ActionExponent.ChatGenerator })]
    public class GetGenerateByIdQuery : BaseQuery<ChatGeneratorDto>
    {
        public string Id { get; }

        public GetGenerateByIdQuery(string id)
        {
            Id = id;
        }
    }
}
