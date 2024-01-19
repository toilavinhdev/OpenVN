using Microsoft.Extensions.Localization;
using OpenVN.Application.Properties;
using SharedKernel.Runtime.Exceptions;
using static SharedKernel.Application.Enum;
using Directory = OpenVN.Domain.Directory;

namespace OpenVN.Application
{
    public class SetPasswordCommandHandler : BaseCommandHandler, IRequestHandler<SetPasswordCommand, Unit>
    {
        private readonly IStringLocalizer<Resources> _localizer;
        private readonly IDirectoryReadOnlyRepository _directoryReadOnlyRepository;
        private readonly ILockedDirectoryWriteOnlyRepository _lockedDirectoryWriteOnlyRepository;

        public SetPasswordCommandHandler(
            IEventDispatcher eventDispatcher,
            IAuthService authService,
            IStringLocalizer<Resources> localizer,
            IDirectoryReadOnlyRepository directoryReadOnlyRepository,
            ILockedDirectoryWriteOnlyRepository lockedDirectoryWriteOnlyRepository
        ) : base(eventDispatcher, authService)
        {
            _localizer = localizer;
            _directoryReadOnlyRepository = directoryReadOnlyRepository;
            _lockedDirectoryWriteOnlyRepository = lockedDirectoryWriteOnlyRepository;
        }

        public async Task<Unit> Handle(SetPasswordCommand request, CancellationToken cancellationToken)
        {
            var directoryId = request.SetPasswordDto.DirectoryId;
            var password = request.SetPasswordDto.Password;

            await CheckDirectoryAndThrowAsync(directoryId, cancellationToken);

            var hasPassword = await _directoryReadOnlyRepository.HasPasswordAsync(long.Parse(directoryId), cancellationToken);
            if (hasPassword)
            {
                throw new BadRequestException(_localizer["cloud_directory_already_exist_password"]);
            }

            await _lockedDirectoryWriteOnlyRepository.SetPasswordAsync(long.Parse(directoryId), password, cancellationToken);
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
