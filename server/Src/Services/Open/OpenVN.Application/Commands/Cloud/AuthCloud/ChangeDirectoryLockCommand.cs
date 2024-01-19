using SharedKernel.Libraries;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    [AuthorizationRequest(new ActionExponent[] { ActionExponent.Cloud })]
    public class ChangeDirectoryLockCommand : BaseUpdateCommand
    {
        public string DirectoryId { get; }
        public string Type { get; }
        public string Password { get; }


        public ChangeDirectoryLockCommand(string directoryId, string type, string password)
        {
            DirectoryId = directoryId;
            Type = type;
            Password = password;

        }
    }
}
