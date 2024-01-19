using AutoMapper;
using Microsoft.Extensions.Localization;
using OpenVN.Application.Properties;

namespace OpenVN.Application
{
    public class GetUserInformationQueryHandler : BaseQueryHandler, IRequestHandler<GetUserInformationQuery, UserDto>
    {
        private readonly IUserReadOnlyRepository _userReadOnlyRepository;
        private readonly IStringLocalizer<Resources> _localizer;

        public GetUserInformationQueryHandler(
            IAuthService authService,
            IUserReadOnlyRepository userReadOnlyRepository,
            IMapper mapper,
            IStringLocalizer<Resources> localizer
        ) : base(authService, mapper)
        {
            _userReadOnlyRepository = userReadOnlyRepository;
            _localizer = localizer;
        }

        public async Task<UserDto> Handle(GetUserInformationQuery request, CancellationToken cancellationToken)
        {
            var user = await _userReadOnlyRepository.GetUserInformationAsync(cancellationToken);
            if (user == null)
            {
                throw new BadRequestException(_localizer["user_does_not_exists"]);
            }
            return _mapper.Map<UserDto>(user);
        }
    }
}
