using AutoMapper;
using Microsoft.Extensions.Localization;
using OpenVN.Application.Properties;
using Directory = OpenVN.Domain.Directory;

namespace OpenVN.Application
{
    public class GetDirectoryPathQueryHandler : BaseQueryHandler, IRequestHandler<GetDirectoryPathQuery, PathDto>
    {
        private readonly IDirectoryReadOnlyRepository _directoryReadOnlyRepository;
        private readonly IStringLocalizer<Resources> _localizer;

        public GetDirectoryPathQueryHandler(
            IAuthService authService,
            IMapper mapper,
            IDirectoryReadOnlyRepository directoryReadOnlyRepository,
            IStringLocalizer<Resources> localizer
        ) : base(authService, mapper)
        {
            _directoryReadOnlyRepository = directoryReadOnlyRepository;
            _localizer = localizer;
        }

        public async Task<PathDto> Handle(GetDirectoryPathQuery request, CancellationToken cancellationToken)
        {
            if (request == null || !long.TryParse(request.DirectoryId, out var dirId) || dirId < 0)
            {
                throw new BadRequestException(_localizer["common_payload_is_not_valid"]);
            }
            if (dirId == 0)
            {
                return new PathDto();
            }

            await CheckDirectoryAsync(dirId, cancellationToken);
            return await _directoryReadOnlyRepository.GetPathAsync(dirId, cancellationToken);
        }

        private async Task CheckDirectoryAsync(long directoryId, CancellationToken cancellationToken)
        {
            if (directoryId > 0)
            {
                var dir = await _directoryReadOnlyRepository.GetByIdAsync<Directory>(directoryId.ToString(), cancellationToken);
                if (dir == null)
                {
                    throw new BadRequestException(_localizer["common_data_does_not_exist_or_was_deleted"]);
                }
            }
        }
    }
}
