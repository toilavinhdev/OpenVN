using Microsoft.Extensions.Localization;
using OpenVN.Application.Properties;
using SharedKernel.Runtime.Exceptions;

namespace OpenVN.Application
{
    public class SignUpCommandHandler : BaseCommandHandler, IRequestHandler<SignUpCommand, string>
    {
        private readonly IUserReadOnlyRepository _userReadOnlyRepository;
        private readonly IUserWriteOnlyRepository _userWriteOnlyRepository;
        private readonly IStringLocalizer<Resources> _localizer;

        public SignUpCommandHandler(IEventDispatcher eventDispatcher, IAuthService authService, IUserReadOnlyRepository userReadOnlyRepository, IUserWriteOnlyRepository userWriteOnlyRepository, IStringLocalizer<Resources> localizer) : base(eventDispatcher, authService)
        {
            _userReadOnlyRepository = userReadOnlyRepository;
            _userWriteOnlyRepository = userWriteOnlyRepository;
            _localizer = localizer;
        }

        public async Task<string> Handle(SignUpCommand request, CancellationToken cancellationToken)
        {
            await ValidateAndThrowAsync(request.CreateUserDto, cancellationToken);
            return (await _userWriteOnlyRepository.CreateUserAsync(request.CreateUserDto, cancellationToken)).ToString();
        }

        private async Task ValidateAndThrowAsync(CreateUserDto dto, CancellationToken cancellationToken)
        {
            var type = await _userReadOnlyRepository.CheckDuplicateAsync(dto.Username, dto.Email, dto.PhoneNumber, 0, cancellationToken);
            if (!string.IsNullOrEmpty(type))
            {
                switch (type)
                {
                    case "username":
                        throw new BadRequestException(_localizer["auth_user_already_exist"].Value);
                    case "email":
                        throw new BadRequestException(_localizer["auth_email_already_exist"].Value);
                    //case "phone":
                    //    throw new BadRequestException(_localizer["auth_phone_already_exist"].Value); // NEED_REFACTOR
                    default:
                        break;
                }
            }
        }
    }
}
