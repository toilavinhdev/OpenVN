using AutoMapper;
using Microsoft.Extensions.Localization;
using OpenVN.Application.Properties;

namespace OpenVN.Application
{
    public class GetFilePropertiesQueryHandler : BaseQueryHandler, IRequestHandler<GetFilePropertiesQuery, FilePropertyDto>
    {
        private readonly ICloudFileReadOnlyRepository _cloudFileReadOnlyRepository;
        private readonly IStringLocalizer<Resources> _localizer;

        public GetFilePropertiesQueryHandler(
            IAuthService authService,
            IMapper mapper,
            ICloudFileReadOnlyRepository cloudFileReadOnlyRepository,
            IStringLocalizer<Resources> localizer
        ) : base(authService, mapper)
        {
            _cloudFileReadOnlyRepository = cloudFileReadOnlyRepository;
            _localizer = localizer;
        }

        public async Task<FilePropertyDto> Handle(GetFilePropertiesQuery request, CancellationToken cancellationToken)
        {
            if (!long.TryParse(request.FileId, out var id) || id <= 0)
            {
                throw new BadRequestException(_localizer["cloud_file_id_is_invalid"]);
            }

            var properties = await _cloudFileReadOnlyRepository.GetPropertiesAsync(id, cancellationToken);
            return properties;
        }
    }
}
