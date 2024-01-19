using AutoMapper;
using Microsoft.Extensions.Localization;
using OpenVN.Application.Properties;
using SharedKernel.Auth;
using SharedKernel.Core;
using SharedKernel.Libraries;
using SharedKernel.Providers;
using SharedKernel.Runtime.Exceptions;
using static SharedKernel.Application.Enum;
using Directory = OpenVN.Domain.Directory;

namespace OpenVN.Application
{
    public class UploadCommandHandler : BaseCommandHandler, IRequestHandler<UploadCommand, List<CloudFileDto>>
    {
        private readonly IStringLocalizer<Resources> _localizer;
        private readonly ICloudConfigService _cloudConfigService;
        private readonly IDirectoryReadOnlyRepository _directoryReadOnlyRepository;
        private readonly ICloudConfigReadOnlyRepository _cloudConfigReadOnlyRepository;
        private readonly ICloudFileReadOnlyRepository _cloudFileReadOnlyRepository;
        private readonly ICloudFileWriteOnlyRepository _cloudFileWriteOnlyRepository;
        private readonly ILockDirectoryService _lockDirectoryService;
        private readonly IS3StorageProvider _s3;
        private readonly IMapper _mapper;
        private readonly IToken _token;

        public UploadCommandHandler(
            IEventDispatcher eventDispatcher,
            IAuthService authService,
            IStringLocalizer<Resources> localizer,
            ICloudConfigService cloudConfigService,
            IDirectoryReadOnlyRepository directoryReadOnlyRepository,
            ICloudConfigReadOnlyRepository cloudConfigReadOnlyRepository,
            ICloudFileReadOnlyRepository cloudFileReadOnlyRepository,
            ICloudFileWriteOnlyRepository cloudFileWriteOnlyRepository,
            ILockDirectoryService lockDirectoryService,
            IS3StorageProvider s3,
            IMapper mapper,
            IToken token
        ) : base(eventDispatcher, authService)
        {
            _localizer = localizer;
            _cloudConfigService = cloudConfigService;
            _directoryReadOnlyRepository = directoryReadOnlyRepository;
            _cloudConfigReadOnlyRepository = cloudConfigReadOnlyRepository;
            _cloudFileReadOnlyRepository = cloudFileReadOnlyRepository;
            _cloudFileWriteOnlyRepository = cloudFileWriteOnlyRepository;
            _lockDirectoryService = lockDirectoryService;
            _s3 = s3;
            _mapper = mapper;
            _token = token;
        }

        public async Task<List<CloudFileDto>> Handle(UploadCommand request, CancellationToken cancellationToken)
        {
            // Check file extension
            CheckAcceptFileExtensionAndThrow(request);

            // Check directory if exist
            var directory = await CheckDirectoryAndThrowAsync(request, cancellationToken);

            // Check locked directory
            await _lockDirectoryService.MakeSureLockedDirectoryIsSafeAsync(long.Parse(request.DirectoryId), cancellationToken: cancellationToken);

            // Check avaiable capacity
            await CheckAvaiableCapacityAndThrowAsync(request, cancellationToken);

            // Đẩy files lên s3
            var uploadRequests = request.Files.Select(f => new UploadRequest
            {
                FileName = f.FileName,
                FileExtension = Path.GetExtension(f.FileName).ToLower(),
                Size = f.Length,
                Stream = f.OpenReadStream(),
            }).ToList();

            var uploadResponses = await _s3.UploadAsync(uploadRequests, cancellationToken);
            var successFiles = uploadResponses.Where(r => r.Success).ToList();

            // Đẩy files failed
            if (!successFiles.Any())
            {
                throw new CatchableException(_localizer["cloud_upload_failed"]);
            }

            // Lưu lại files theo file name mới trả vê từ s3
            var files = successFiles.Select(f => new CloudFile(f.CurrentFileName, f.OriginalFileName, f.FileExtension, f.Size, long.Parse(request.DirectoryId)))
                                    .ToList();

            var fileResults = await _cloudFileWriteOnlyRepository.SaveAsync(files, cancellationToken);
            await _cloudFileWriteOnlyRepository.UnitOfWork.CommitAsync(cancellationToken: cancellationToken);

            // Fire events
            var models = new List<UploadFileAuditModel>();
            foreach (var file in files)
            {
                models.Add(new UploadFileAuditModel(file, directory));
            }
            _ = _eventDispatcher.FireEvent(new UploadFileAuditEvent(models, _token), cancellationToken);

            // Map lại orginal file name
            var result = _mapper.Map<List<CloudFileDto>>(fileResults);
            var mapping = new Dictionary<string, string>();

            uploadResponses.ForEach(u => mapping[u.CurrentFileName] = u.OriginalFileName);
            result.ForEach(r => r.OriginalFileName = mapping[r.FileName]);

            return result;
        }

        private void CheckAcceptFileExtensionAndThrow(UploadCommand request)
        {
            if (DefaultS3Config.AcceptExtensions.Contains("*"))
            {
                return;
            }
            foreach (var file in request.Files)
            {
                if (!DefaultS3Config.AcceptExtensions.Contains(Path.GetExtension(file.FileName).ToLower()))
                {
                    throw new BadRequestException($"File extension is not valid: {file.FileName}");
                }
            }
        }

        private async Task<Directory> CheckDirectoryAndThrowAsync(UploadCommand request, CancellationToken cancellationToken)
        {
            if (!long.TryParse(request.DirectoryId, out var directoryId) || directoryId < 0)
            {
                throw new BadRequestException(_localizer["directory_id_is_invalid"]);
            }

            Directory directory = null;
            if (directoryId > 0)
            {
                directory = await _directoryReadOnlyRepository.GetByIdAsync<Directory>(request.DirectoryId, cancellationToken);
                if (directory == null)
                {
                    throw new BadRequestException(_localizer["directory_not_found"]);
                }
            }
            return directory;
        }

        private async Task CheckAvaiableCapacityAndThrowAsync(UploadCommand request, CancellationToken cancellationToken)
        {
            var config = await _cloudConfigService.GetCapacityConfigurationAsync(cancellationToken);
            foreach (var file in request.Files)
            {
                if (file.Length > config.MaxFileSize)
                {
                    throw new BadRequestException(_localizer["cloud_over_max_file_size", config.MaxFileSizeText]);
                }
            }

            var totalFileSize = request.Files.Sum(f => f.Length);

            if (totalFileSize > config.AvailableCapacity)
            {
                throw new BadRequestException(_localizer["cloud_not_enough_capacity", config.AvailableCapacity == 0 ? "0" : string.Format("{0:0.00}", FileHelper.ConvertBytesToMegabytes(config.AvailableCapacity))]);
            }
        }
    }
}
