using AutoMapper;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using SharedKernel.Auth;
using SharedKernel.Caching;
using SharedKernel.Providers;

namespace OpenVN.Application
{
    public class UserService : IUserService
    {
        private readonly IUserReadOnlyRepository _userReadOnlyRepository;
        private readonly ISequenceCaching _caching;
        private readonly IS3StorageProvider _s3;

        public UserService(
            IUserReadOnlyRepository userReadOnlyRepository,
            ISequenceCaching caching,
            IS3StorageProvider s3
        )
        {
            _userReadOnlyRepository = userReadOnlyRepository;
            _caching = caching;
            _s3 = s3;
        }

        public async Task<string> GetAvatarUrlByFileNameAsync(string fileName, object tenantId, object ownerId, CancellationToken cancellationToken)
        {
            var cacheKey = OpenCacheKeys.GetAvatarUrlKey(tenantId, ownerId);
            var url = await _caching.GetStringAsync(cacheKey, cancellationToken: cancellationToken);
            if (string.IsNullOrEmpty(url))
            {
                if (string.IsNullOrEmpty(fileName))
                {
                    var avatar = await _userReadOnlyRepository.GetAvatarAsync(cancellationToken);
                    if (avatar == null)
                    {
                        return string.Empty;
                    }
                    fileName = avatar.FileName;
                }

                var response = await _s3.DownloadAsync(fileName, cancellationToken: cancellationToken);
                if (response != null)
                {
                    url = response.PresignedUrl;
                    await _caching.SetAsync(cacheKey, url, TimeSpan.FromDays(30), cancellationToken: cancellationToken);
                }
            }
            return url;
        }
    }
}
