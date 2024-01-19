using AutoMapper;
using Microsoft.Extensions.Localization;
using OpenVN.Application.Properties;

namespace OpenVN.Application
{
    public class GetDirectoryPropertiesQueryHandler : BaseQueryHandler, IRequestHandler<GetDirectoryPropertiesQuery, DirectoryPropertyDto>
    {
        private readonly IStringLocalizer<Resources> _localizer;
        private readonly IDirectoryReadOnlyRepository _directoryReadOnlyRepository;

        public GetDirectoryPropertiesQueryHandler(
            IAuthService authService,
            IMapper mapper,
            IStringLocalizer<Resources> localizer,
            IDirectoryReadOnlyRepository directoryReadOnlyRepository
        ) : base(authService, mapper)
        {
            _localizer = localizer;
            _directoryReadOnlyRepository = directoryReadOnlyRepository;
        }

        public async Task<DirectoryPropertyDto> Handle(GetDirectoryPropertiesQuery request, CancellationToken cancellationToken)
        {
            if (!long.TryParse(request.Id, out var id) || id <= 0)
            {
                throw new BadRequestException(_localizer["cloud_directory_id_is_invalid"]);
            }

            var properties = await _directoryReadOnlyRepository.GetPropertiesAsync(id, cancellationToken);
            return properties;
        }
    }
}
