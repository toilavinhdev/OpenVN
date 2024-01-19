using SharedKernel.Libraries;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    [AuthorizationRequest(new ActionExponent[] { ActionExponent.Cloud })]
    public class ExtendCodeCommand : BaseUpdateCommand
    {
        public string DirectoryId { get; set; }

        public string Code { get; }

        public ExtendCodeCommand(string directoryId, string code)
        {
            DirectoryId = directoryId;
            Code = code;
        }
    }
}
