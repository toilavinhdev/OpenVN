using AutoMapper;
using Microsoft.Extensions.Localization;
using OpenVN.Application.Properties;
using SharedKernel.Auth;
using SharedKernel.Runtime.Exceptions;
using System.Linq;
using System.Threading;
using static Microsoft.IO.RecyclableMemoryStreamManager;
using static SharedKernel.Application.Enum;
using Directory = OpenVN.Domain.Directory;

namespace OpenVN.Application
{
    public class DeleteDirectoryCommandHandler : BaseCommandHandler, IRequestHandler<DeleteDirectoryCommand, Unit>
    {
        private readonly IDirectoryReadOnlyRepository _directoryReadOnlyRepository;
        private readonly IDirectoryWriteOnlyRepository _directoryWriteOnlyRepository;
        private readonly ICloudFileReadOnlyRepository _cloudFileReadOnlyRepository;
        private readonly IMapper _mapper;
        private readonly IToken _token;
        private readonly IStringLocalizer<Resources> _localizer;

        public DeleteDirectoryCommandHandler(
            IEventDispatcher eventDispatcher,
            IAuthService authService,
            IDirectoryReadOnlyRepository directoryReadOnlyRepository,
            IDirectoryWriteOnlyRepository directoryWriteOnlyRepository,
            ICloudFileReadOnlyRepository cloudFileReadOnlyRepository,
            IMapper mapper,
            IToken token,
            IStringLocalizer<Resources> localizer
        ) : base(eventDispatcher, authService)
        {
            _directoryReadOnlyRepository = directoryReadOnlyRepository;
            _directoryWriteOnlyRepository = directoryWriteOnlyRepository;
            _cloudFileReadOnlyRepository = cloudFileReadOnlyRepository;
            _mapper = mapper;
            _token = token;
            _localizer = localizer;
        }

        public async Task<Unit> Handle(DeleteDirectoryCommand request, CancellationToken cancellationToken)
        {
            if (request.Ids == null || !request.Ids.Any())
            {
                throw new BadRequestException(_localizer["common_list_id_must_not_be_empty"]);
            }
            var ids = request.Ids.Select(id => long.Parse(id)).ToList();
            var directories = await _directoryWriteOnlyRepository.DeleteAsync(request.Ids, cancellationToken);
            var files = await _cloudFileReadOnlyRepository.GetFilesInDirectoriesAsync(directories.Select(x => x.Id).ToList(), cancellationToken);

            await _directoryWriteOnlyRepository.UnitOfWork.CommitAsync(cancellationToken: cancellationToken);

            FireEvents(directories, files, cancellationToken);

            return Unit.Value;
        }

        private void FireEvents(List<Directory> directories, List<CloudFile> files, CancellationToken cancellationToken)
        {
            var dirModels = new List<DeleteDirectoryAuditModel>();
            var fileModels = new List<DeleteFileAuditModel>();

            foreach (var directory in directories)
            {
                var parent = directories.FirstOrDefault(x => x.Id == directory.ParentId);
                dirModels.Add(new DeleteDirectoryAuditModel(directory, parent, parent != null));
            }
            if (dirModels.Any())
            {
                _ = _eventDispatcher.FireEvent(new DeleteDirectoryAuditEvent(dirModels, _token), cancellationToken);
            }

            foreach (var file in files)
            {
                var directory = directories.FirstOrDefault(x => x.Id == file.DirectoryId);
                var clone = (CloudFile)file.Clone();
                clone.ClearDomainEvents();

                fileModels.Add(new DeleteFileAuditModel(clone, directory));
            }
            if (fileModels.Any())
            {
                _ = _eventDispatcher.FireEvent(new DeleteFileAuditEvent(fileModels, _token), cancellationToken);
            }
        }
    }
}
