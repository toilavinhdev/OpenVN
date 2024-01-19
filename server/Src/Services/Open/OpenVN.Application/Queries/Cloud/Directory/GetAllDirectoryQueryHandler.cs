using AutoMapper;
using Microsoft.Extensions.Localization;
using OpenVN.Application.Properties;
using Directory = OpenVN.Domain.Directory;

namespace OpenVN.Application
{
    public class GetAllDirectoryQueryHandler : BaseQueryHandler, IRequestHandler<GetAllDirectoryQuery, List<DirectoryDto>>
    {
        private readonly IDirectoryReadOnlyRepository _directoryReadOnlyRepository;
        private readonly IStringLocalizer<Resources> _localizer;
        private readonly ILockDirectoryService _lockDirectoryService;

        public GetAllDirectoryQueryHandler(
            IAuthService authService,
            IMapper mapper,
            IDirectoryReadOnlyRepository directoryReadOnlyRepository,
            IStringLocalizer<Resources> localizer,
            ILockDirectoryService lockDirectoryService
        ) : base(authService, mapper)
        {
            _directoryReadOnlyRepository = directoryReadOnlyRepository;
            _localizer = localizer;
            _lockDirectoryService = lockDirectoryService;
        }

        public async Task<List<DirectoryDto>> Handle(GetAllDirectoryQuery request, CancellationToken cancellationToken)
        {
            if (request == null || !long.TryParse(request.DirectoryId, out var dirId) || dirId < 0)
            {
                throw new BadRequestException(_localizer["common_payload_is_not_valid"]);
            }

            await CheckDirectoryAsync(dirId, cancellationToken);
            await _lockDirectoryService.MakeSureLockedDirectoryIsSafeAsync(dirId, cancellationToken: cancellationToken);

            return await _directoryReadOnlyRepository.GetChildrenDirectoryAsync(dirId, cancellationToken);
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
