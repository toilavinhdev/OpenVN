using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;
using OpenVN.Application.Properties;
using SharedKernel.Auth;
using SharedKernel.Caching;
using SharedKernel.Providers;
using SharedKernel.SignalR;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    public class GetFilesInDirQueryHandler : BaseQueryHandler, IRequestHandler<GetFilesInDirQuery, List<CloudFileDto>>
    {
        private readonly ICloudFileReadOnlyRepository _cloudFileReadOnlyRepository;
        private readonly IStringLocalizer<Resources> _localizer;
        private readonly IS3StorageProvider _s3;
        private readonly IToken _token;
        private readonly ISequenceCaching _caching;
        private readonly ILockDirectoryService _lockDirectoryService;
        private readonly IOpenMessageHub _hub;

        public GetFilesInDirQueryHandler(
            IAuthService authService,
            IMapper mapper,
            ICloudFileReadOnlyRepository cloudFileReadOnlyRepository,
            IStringLocalizer<Resources> localizer,
            IS3StorageProvider s3,
            IToken token,
            ISequenceCaching caching,
            ILockDirectoryService lockDirectoryService,
            IOpenMessageHub hub
        ) : base(authService, mapper)
        {
            _cloudFileReadOnlyRepository = cloudFileReadOnlyRepository;
            _localizer = localizer;
            _s3 = s3;
            _token = token;
            _caching = caching;
            _lockDirectoryService = lockDirectoryService;
            _hub = hub;
        }
        public async Task<List<CloudFileDto>> Handle(GetFilesInDirQuery request, CancellationToken cancellationToken)
        {
            if (request == null || !long.TryParse(request.DirectoryId, out var directoryId) || directoryId < 0)
            {
                throw new BadRequestException(_localizer["common_payload_is_not_valid"]);
            }

            await _lockDirectoryService.MakeSureLockedDirectoryIsSafeAsync(directoryId, cancellationToken: cancellationToken);

            var result = await _cloudFileReadOnlyRepository.GetFilesInDirectoryAsync(directoryId, cancellationToken);
            var tasks = new List<Task<DownloadResponse>>();
            foreach (var cfd in result)
            {
                var key = OpenCacheKeys.GetCloudFileUrlKey(_token.Context.TenantId, _token.Context.OwnerId, cfd.FileName);
                var data = await _caching.GetStringAsync(key, cancellationToken: cancellationToken);
                if (string.IsNullOrEmpty(data))
                {
                    tasks.Add(_s3.DownloadAsync(cfd.FileName, cancellationToken: cancellationToken));
                }
                else
                {
                    cfd.Url = data;
                }

            }
            var responses = (await Task.WhenAll(tasks)).ToList();
            foreach (var resp in responses)
            {
                result.Find(x => x.FileName.Equals(resp.FileName)).Url = resp.PresignedUrl;

                var key = OpenCacheKeys.GetCloudFileUrlKey(_token.Context.TenantId, _token.Context.OwnerId, resp.FileName);
                _ = _caching.SetAsync(key, resp.PresignedUrl, TimeSpan.FromDays(365), cancellationToken: cancellationToken);
            }
            result.Sort((a, b) =>
            {
                bool isImageA = a.FileType == FileType.Image;
                bool isVideoA = a.FileType == FileType.Video;
                bool isImageB = b.FileType == FileType.Image;
                bool isVideoB = b.FileType == FileType.Video;
                bool isOtherA = !isImageA && !isVideoA;
                bool isOtherB = !isImageB && !isVideoB;

                if (isOtherA && isOtherB)
                    return 0;
                if (isOtherA && !isOtherB)
                    return -1;
                if (!isOtherA && isOtherB)
                    return 1;
                if (isImageA && isVideoB)
                    return -1;
                if (isVideoA && isImageB)
                    return 1;

                return 0;
            });

            //var batches = result.Chunk(10);
            //_ = Task.Run(async () =>
            //{
            //    foreach (var batch in batches)
            //    {
            //        var message = new MessageHubResponse { Type = MessageHubType.ReceivedFile, Message = batch };
            //        await _hub.HubContext.Clients.Clients(request.ConnectionId).SendAsync("ReceiveMessage", message);
            //        await Task.Delay(500);
            //    }
            //});

            return result;
        }
    }
}
