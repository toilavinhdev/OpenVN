using Microsoft.Extensions.Localization;
using OpenVN.Application.Properties;
using SharedKernel.Auth;
using SharedKernel.Domain;
using SharedKernel.Runtime.Exceptions;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    public class DeleteCloudFileCommandHandler : BaseCommandHandler, IRequestHandler<DeleteCloudFileCommand, Unit>
    {
        private readonly IStringLocalizer<Resources> _localizer;
        private readonly ICloudFileWriteOnlyRepository _cloudFileWriteOnlyRepository;
        private readonly ILockDirectoryService _lockDirectoryService;
        private readonly IToken _token;

        public DeleteCloudFileCommandHandler(
            IEventDispatcher eventDispatcher,
            IAuthService authService,
            IStringLocalizer<Resources> localizer,
            ICloudFileWriteOnlyRepository cloudFileWriteOnlyRepository,
            ILockDirectoryService lockDirectoryService,
            IToken token
        ) : base(eventDispatcher, authService)
        {
            _localizer = localizer;
            _cloudFileWriteOnlyRepository = cloudFileWriteOnlyRepository;
            _lockDirectoryService = lockDirectoryService;
            _token = token;
        }

        public async Task<Unit> Handle(DeleteCloudFileCommand request, CancellationToken cancellationToken)
        {
            if (!long.TryParse(request.DirectoryId, out var directoryId) || directoryId < 0)
            {
                throw new BadRequestException(_localizer["directory_id_is_invalid"]);
            }

            if (request.Ids == null || !request.Ids.Any())
            {
                throw new BadRequestException(_localizer["common_list_id_must_not_be_empty"]);
            }

            // Check locked directory
            await _lockDirectoryService.MakeSureLockedDirectoryIsSafeAsync(directoryId, cancellationToken: cancellationToken);

            var entities = await _cloudFileWriteOnlyRepository.DeleteAsync(request.Ids, cancellationToken);
            await _cloudFileWriteOnlyRepository.UnitOfWork.CommitAsync(cancellationToken: cancellationToken);

            return Unit.Value;
        }
    }
}
