using SharedKernel.Libraries;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    [AuthorizationRequest(new ActionExponent[] { ActionExponent.AllowAnonymous })]
    public class SignUpCommand : BaseInsertCommand<string>
    {
        public CreateUserDto CreateUserDto { get; }

        public SignUpCommand(CreateUserDto createUserDto)
        {
            CreateUserDto = createUserDto;
        }
    }
}
