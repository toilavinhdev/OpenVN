using Microsoft.AspNetCore.Http;
using SharedKernel.Libraries;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    [AuthorizationRequest(new ActionExponent[] { ActionExponent.ChatGenerator })]
    public class AnalyseTextCommand : BaseInsertCommand<ChatContentDto>
    {
        public IFormFile File { get; }

        public AnalyseTextCommand(IFormFile file)
        {
            File = file;
        }
    }
}
