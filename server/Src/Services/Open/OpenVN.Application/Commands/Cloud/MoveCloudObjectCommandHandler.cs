using Microsoft.Extensions.Localization;
using OpenVN.Application.Properties;
using SharedKernel.Auth;
using SharedKernel.Runtime.Exceptions;
using static SharedKernel.Application.Enum;
using Directory = OpenVN.Domain.Directory;

namespace OpenVN.Application
{
    public class MoveCloudObjectCommandHandler : BaseCommandHandler, IRequestHandler<MoveCloudObjectCommand, Unit>
    {
        private readonly IStringLocalizer<Resources> _localizer;
        private readonly IDirectoryReadOnlyRepository _directoryReadOnlyRepository;
        private readonly ICloudFileReadOnlyRepository _cloudFileReadOnlyRepository;
        private readonly IMoveCloudObjectRepository _moveCloudObjectRepository;
        private readonly ILockDirectoryService _lockDirectoryService;
        private readonly IToken _token;

        public MoveCloudObjectCommandHandler(
            IEventDispatcher eventDispatcher,
            IAuthService authService,
            IStringLocalizer<Resources> localizer,
            IDirectoryReadOnlyRepository directoryReadOnlyRepository,
            ICloudFileReadOnlyRepository cloudFileReadOnlyRepository,
            IMoveCloudObjectRepository moveCloudObjectRepository,
            ILockDirectoryService lockDirectoryService,
            IToken token
        ) : base(eventDispatcher, authService)
        {
            _localizer = localizer;
            _directoryReadOnlyRepository = directoryReadOnlyRepository;
            _cloudFileReadOnlyRepository = cloudFileReadOnlyRepository;
            _moveCloudObjectRepository = moveCloudObjectRepository;
            _lockDirectoryService = lockDirectoryService;
            _token = token;
        }

        public async Task<Unit> Handle(MoveCloudObjectCommand request, CancellationToken cancellationToken)
        {
            var destinationId = long.Parse(request.MoveDto.DestinationId);
            var destinationSecretCode = request.MoveDto.DestinationSecretCode;
            var sourceSecretCode = request.MoveDto.SourceSecretCode;
            Directory destination = null;

            if (destinationId > 0)
            {
                destination = await _directoryReadOnlyRepository.GetByIdAsync<Directory>(destinationId, cancellationToken);
                if (destination == null)
                {
                    throw new BadRequestException(_localizer["cloud_destination_does_not_exist_or_was_deleted"]);
                }
                // make sure destination directory is safe if it's locked
                await _lockDirectoryService.MakeSureLockedDirectoryIsSafeAsync(destinationId, destinationSecretCode, UnSafeCallback, cancellationToken);
            }

            var moveObjects = new List<MoveObject>();
            var invalidMoveObjects = new List<MoveObject>();

            for (int i = 0; i < request.MoveDto.MoveObjects.Count; i++)
            {
                var moveObject = request.MoveDto.MoveObjects[i];
                if (moveObject.SourceId.Equals(destinationId.ToString()))
                {
                    continue;
                }

                var relationship = await _directoryReadOnlyRepository.GetRelationshipBetwenTwoDirectories(long.Parse(moveObject.SourceId), destinationId, cancellationToken);
                if (relationship == DirectoryRelationshipType.LeftIsRoot)
                {
                    invalidMoveObjects.Add(moveObject);
                    continue;
                }

                moveObjects.Add(moveObject);
            }

            if (invalidMoveObjects.Count == request.MoveDto.MoveObjects.Count)
            {
                throw new BadRequestException(_localizer["cloud_cannot_move_root_into_node"]);
            }

            if (moveObjects.Any())
            {
                var moveDirectories = new List<Directory>();
                var moveFiles = new List<CloudFile>();
                var directoryIds = moveObjects.Where(x => x.Type.Equals("dir")).Select(x => long.Parse(x.SourceId)).Distinct().ToList();
                var fileIds = moveObjects.Where(x => x.Type.Equals("cf")).Select(x => long.Parse(x.SourceId)).Distinct().ToList();
                var sourceIds = new List<long>();


                if (directoryIds.Any())
                {
                    moveDirectories = (await _directoryReadOnlyRepository.GetListDirectoryByIdsAsync(directoryIds, cancellationToken)).ToList();
                    sourceIds.AddRange(moveDirectories.Select(x => x.ParentId));
                }

                if (fileIds.Any())
                {
                    moveFiles = (await _cloudFileReadOnlyRepository.GetListFileByIdsAsync(fileIds, cancellationToken)).ToList();
                    sourceIds.AddRange(moveFiles.Select(x => x.DirectoryId));
                }

                // make sure all objects are from one source
                if (!sourceIds.Any() || sourceIds.Distinct().Count() > 1)
                {
                    throw new BadRequestException(_localizer["cloud_bad_request"]);
                }

                // make sure source directory is safe if it's locked
                await _lockDirectoryService.MakeSureLockedDirectoryIsSafeAsync(sourceIds.First(), sourceSecretCode, UnSafeCallback, cancellationToken);

                // if all is safe then perform object move
                await _moveCloudObjectRepository.MoveObjectAsync(destinationId, moveObjects, cancellationToken);
                await _moveCloudObjectRepository.UnitOfWork.CommitAsync(cancellationToken: cancellationToken);

                await FireEvents(destination, moveDirectories, moveFiles, cancellationToken);
            }

            return Unit.Value;
        }

        public void UnSafeCallback()
        {
            throw new BadRequestException(_localizer["cloud_bad_request"]);
        }

        private async Task FireEvents(Directory destination, List<Directory> moveDirectories, List<CloudFile> moveFiles, CancellationToken cancellationToken)
        {
            var dirModels = new List<MoveDirectoryAuditModel>();
            var fileModels = new List<MoveFileAuditModel>();

            foreach (var directory in moveDirectories)
            {
                var source = await _directoryReadOnlyRepository.GetByIdAsync<Directory>(directory.ParentId, cancellationToken);
                dirModels.Add(new MoveDirectoryAuditModel(directory, source, destination));
            }
            if (dirModels.Any())
            {
                _ = _eventDispatcher.FireEvent(new MoveDirectoryAuditEvent(dirModels, _token), cancellationToken);
            }

            foreach (var file in moveFiles)
            {
                var source = await _directoryReadOnlyRepository.GetByIdAsync<Directory>(file.DirectoryId, cancellationToken);
                fileModels.Add(new MoveFileAuditModel(file, source, destination));
            }
            if (fileModels.Any())
            {
                _ = _eventDispatcher.FireEvent(new MoveFileAuditEvent(fileModels, _token), cancellationToken);
            }
        }
    }
}
