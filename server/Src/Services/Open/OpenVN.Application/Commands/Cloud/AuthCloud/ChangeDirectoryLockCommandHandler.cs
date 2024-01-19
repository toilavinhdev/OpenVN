using Microsoft.Extensions.Localization;
using OpenVN.Application.Properties;
using SharedKernel.Runtime.Exceptions;
using static SharedKernel.Application.Enum;
using Directory = OpenVN.Domain.Directory;

namespace OpenVN.Application
{
    public class ChangeDirectoryLockCommandHandler : BaseCommandHandler, IRequestHandler<ChangeDirectoryLockCommand, Unit>
    {
        private readonly IStringLocalizer<Resources> _localizer;
        private readonly IDirectoryReadOnlyRepository _directoryReadOnlyRepository;
        private readonly ILockedDirectoryWriteOnlyRepository _lockedDirectoryWriteOnlyRepository;
        private readonly ILockedDirectoryReadOnlyRepository _lockedDirectoryReadOnlyRepository;

        public ChangeDirectoryLockCommandHandler(
            IEventDispatcher eventDispatcher,
            IAuthService authService,
            IStringLocalizer<Resources> localizer,
            IDirectoryReadOnlyRepository directoryReadOnlyRepository,
            ILockedDirectoryWriteOnlyRepository lockedDirectoryWriteOnlyRepository,
            ILockedDirectoryReadOnlyRepository lockedDirectoryReadOnlyRepository
        ) : base(eventDispatcher, authService)
        {
            _localizer = localizer;
            _directoryReadOnlyRepository = directoryReadOnlyRepository;
            _lockedDirectoryWriteOnlyRepository = lockedDirectoryWriteOnlyRepository;
            _lockedDirectoryReadOnlyRepository = lockedDirectoryReadOnlyRepository;
        }

        public async Task<Unit> Handle(ChangeDirectoryLockCommand request, CancellationToken cancellationToken)
        {
            await CheckDirectoryAndThrowAsync(request.DirectoryId, cancellationToken);
            if (request.Type.Equals("unlock"))
            {
                var valid = await _lockedDirectoryReadOnlyRepository.CheckPasswordAsync(long.Parse(request.DirectoryId), request.Password, cancellationToken);
                if (!valid)
                {
                    throw new BadRequestException(_localizer["cloud_directory_password_is_incorrect"]);
                }
            }

            await _lockedDirectoryWriteOnlyRepository.ChangeLockStatusAsync(long.Parse(request.DirectoryId), request.Type, cancellationToken);
            await _lockedDirectoryWriteOnlyRepository.UnitOfWork.CommitAsync(cancellationToken: cancellationToken);

            return Unit.Value;
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
