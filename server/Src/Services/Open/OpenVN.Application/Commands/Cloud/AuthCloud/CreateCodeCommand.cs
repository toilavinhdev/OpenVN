using SharedKernel.Libraries;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    [AuthorizationRequest(new ActionExponent[] { ActionExponent.Cloud })]
    public class CreateCodeCommand : BaseInsertCommand<string>
    {
        public string DirectoryId { get; }

        public string Password { get; }

        public CreateCodeCommand(string directoryId, string password)
        {
            DirectoryId = directoryId;
            Password = password;
        }
    }
}
