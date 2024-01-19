using AutoMapper;
using Microsoft.Extensions.Localization;
using OpenVN.Application.Properties;
using Directory = OpenVN.Domain.Directory;

namespace OpenVN.Application
{
    public class GetDirectoryPagingQueryHandler : BaseQueryHandler, IRequestHandler<GetDirectoryPagingQuery, PagingResult<DirectoryDto>>
    {
        private readonly IDirectoryReadOnlyRepository _directoryReadOnlyRepository;
        private readonly IStringLocalizer<Resources> _localizer;
        private readonly ILockDirectoryService _lockDirectoryService;

        public GetDirectoryPagingQueryHandler(
            IAuthService authService,
            IMapper mapper,
            IDirectoryReadOnlyRepository directoryReadOnlyRepository,
            IStringLocalizer<Resources> localizer,
            ILockDirectoryService lockDirectoryService
        ) : base(authService, mapper)
        {
        }

        public async Task<PagingResult<DirectoryDto>> Handle(GetDirectoryPagingQuery request, CancellationToken cancellationToken)
        {
            if (request == null || !long.TryParse(request.DirectoryId, out var dirId) || dirId < 0)
            {
                throw new BadRequestException(_localizer["common_payload_is_not_valid"]);
            }

            await CheckDirectoryAsync(dirId, cancellationToken);
            await _lockDirectoryService.MakeSureLockedDirectoryIsSafeAsync(dirId, cancellationToken: cancellationToken);

            var result = await _directoryReadOnlyRepository.GetPagingResultAsync(dirId, request.PagingRequest, cancellationToken);
            return new PagingResult<DirectoryDto>
            {
                Data = _mapper.Map<List<DirectoryDto>>(result.Data),
                Count = result.Count
            };
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
