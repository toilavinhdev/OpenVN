using Microsoft.Extensions.Localization;
using OpenVN.Application.Properties;
using SharedKernel.Auth;
using SharedKernel.Caching;
using SharedKernel.Libraries;
using SharedKernel.Runtime.Exceptions;
using static SharedKernel.Application.Enum;
using Directory = OpenVN.Domain.Directory;

namespace OpenVN.Application
{
    public class CreateCodeCommandHandler : BaseCommandHandler, IRequestHandler<CreateCodeCommand, string>
    {
        private readonly ISequenceCaching _caching;
        private readonly IToken _token;
        private readonly IDirectoryReadOnlyRepository _directoryReadOnlyRepository;
        private readonly ILockedDirectoryReadOnlyRepository _lockedDirectoryReadOnlyRepository;
        private readonly IStringLocalizer<Resources> _localizer;

        public CreateCodeCommandHandler(
            IEventDispatcher eventDispatcher,
            IAuthService authService,
            ISequenceCaching caching,
            IToken token,
            IDirectoryReadOnlyRepository directoryReadOnlyRepository,
            ILockedDirectoryReadOnlyRepository lockedDirectoryReadOnlyRepository,
            IStringLocalizer<Resources> localizer
        ) : base(eventDispatcher, authService)
        {
            _caching = caching;
            _token = token;
            _directoryReadOnlyRepository = directoryReadOnlyRepository;
            _lockedDirectoryReadOnlyRepository = lockedDirectoryReadOnlyRepository;
            _localizer = localizer;
        }

        public async Task<string> Handle(CreateCodeCommand request, CancellationToken cancellationToken)
        {
            await CheckDirectoryAndThrowAsync(request.DirectoryId, cancellationToken);

            var valid = await _lockedDirectoryReadOnlyRepository.CheckPasswordAsync(long.Parse(request.DirectoryId), request.Password, cancellationToken);
            if (!valid)
            {
                throw new BadRequestException(_localizer["cloud_directory_password_is_incorrect"]);
            }

            var key = OpenCacheKeys.GetCloudCodeKey(_token.Context.TenantId, _token.Context.OwnerId, request.DirectoryId);
            var code = await _caching.GetStringAsync(key, cancellationToken: cancellationToken);
            if (string.IsNullOrEmpty(code))
            {
                code = Utility.RandomString(8);
            }
            await _caching.SetAsync(key, code, TimeSpan.FromMinutes(15), cancellationToken: cancellationToken);
            return code;
        }

        private async Task CheckDirectoryAndThrowAsync(string directoryId, CancellationToken cancellationToken)
        {
            if (!long.TryParse(directoryId, out var dirId) || dirId < 0)
            {
                throw new BadRequestException(_localizer["directory_id_is_invalid"]);
            }

            if (dirId > 0)
            {
                var dir = await _directoryReadOnlyRepository.GetByIdAsync<Directory>(directoryId, cancellationToken);
                if (dir == null)
                {
                    throw new BadRequestException(_localizer["directory_not_found"]);
                }
            }
        }
    }
}
