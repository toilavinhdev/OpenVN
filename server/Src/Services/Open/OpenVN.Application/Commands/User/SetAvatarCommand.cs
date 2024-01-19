using Microsoft.AspNetCore.Http;

namespace OpenVN.Application
{
    public class SetAvatarCommand : BaseUpdateCommand<Unit>
    {
        public IFormFile Avatar { get; }

        public SetAvatarCommand(IFormFile avatar)
        {
            Avatar = avatar;
        }
    }
}
