using AutoMapper;
using Microsoft.Extensions.Localization;
using OpenVN.Application.Properties;

namespace OpenVN.Application
{
    public class GetChildrenNodeDirectoryQueryHandler : BaseQueryHandler, IRequestHandler<GetChildrenNodeDirectoryQuery, List<string>>
    {
        private readonly IDirectoryReadOnlyRepository _directoryReadOnlyRepository;
        private readonly ILockDirectoryService _lockDirectoryService;
        private readonly IStringLocalizer<Resources> _localizer;

        public GetChildrenNodeDirectoryQueryHandler(
            IAuthService authService,
            IMapper mapper,
            IDirectoryReadOnlyRepository directoryReadOnlyRepository,
            ILockDirectoryService lockDirectoryService,
            IStringLocalizer<Resources> localizer
        ) : base(authService, mapper)
        {
            _directoryReadOnlyRepository = directoryReadOnlyRepository;
            _lockDirectoryService = lockDirectoryService;
            _localizer = localizer;
        }

        public async Task<List<string>> Handle(GetChildrenNodeDirectoryQuery request, CancellationToken cancellationToken)
        {
            if (!long.TryParse(request.RootId, out var directoryId) || directoryId < 0)
            {
                throw new BadRequestException(_localizer["directory_id_is_invalid"]);
            }

            await _lockDirectoryService.MakeSureLockedDirectoryIsSafeAsync(directoryId, cancellationToken: cancellationToken);

            var result = await _directoryReadOnlyRepository.GetChildrenNodeIdAsync(directoryId, cancellationToken);
            return result.ToList();
        }
    }
}
