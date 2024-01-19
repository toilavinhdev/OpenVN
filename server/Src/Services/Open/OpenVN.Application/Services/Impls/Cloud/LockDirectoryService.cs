using Microsoft.Extensions.Localization;
using OpenVN.Application.Properties;
using SharedKernel.Auth;
using SharedKernel.Caching;
using SharedKernel.Runtime.Exceptions;
using Action = System.Action;

namespace OpenVN.Application
{
    public class LockDirectoryService : ILockDirectoryService
    {
        private readonly IDirectoryReadOnlyRepository _directoryReadOnlyRepository;
        private readonly IToken _token;
        private readonly ISequenceCaching _sequenceCaching;
        private readonly IStringLocalizer<Resources> _localizer;

        public LockDirectoryService(
            IDirectoryReadOnlyRepository directoryReadOnlyRepository,
            IToken token,
            ISequenceCaching sequenceCaching,
            IStringLocalizer<Resources> localizer
        )
        {
            _directoryReadOnlyRepository = directoryReadOnlyRepository;
            _token = token;
            _sequenceCaching = sequenceCaching;
            _localizer = localizer;
        }

        public async Task MakeSureLockedDirectoryIsSafeAsync(long directoryId, string code = "", Action callback = default, CancellationToken cancellationToken = default)
        {
            if (directoryId > 0)
            {
                var needCheckId = 0L;
                var dirName = "";
                var isLocked = await _directoryReadOnlyRepository.IsLockedDirectoryAsync(directoryId, cancellationToken);

                if (isLocked)
                {
                    needCheckId = directoryId;
                }
                else
                {
                    var check = await _directoryReadOnlyRepository.FindFirstNodeLockedDirectoryAsync(directoryId, cancellationToken);
                    if (check != null)
                    {
                        needCheckId = check.Id;
                        dirName = check.Name;
                    }
                }

                if (needCheckId > 0)
                {
                    if (string.IsNullOrEmpty(code))
                    {
                        code = _token.Context.HttpContext.Request.Headers[HeaderNamesExtension.SecretKey].ToString();
                    }

                    if (string.IsNullOrEmpty(code))
                    {
                        HandleUnSafeRequest(callback, isLocked ? null : new { Key = needCheckId.ToString(), Name = dirName });
                    }

                    var key = OpenCacheKeys.GetCloudCodeKey(_token.Context.TenantId, _token.Context.OwnerId, needCheckId);
                    var value = await _sequenceCaching.GetStringAsync(key);
                    if (!code.Equals(value))
                    {
                        HandleUnSafeRequest(callback, new { Key = needCheckId.ToString(), Name = dirName });
                    }
                }
            }
        }

        private void HandleUnSafeRequest(Action callback, object payload)
        {
            if (callback == null)
            {
                throw new BadRequestException(_localizer["cloud_secret_code_is_incorrect"], BadRequestType.CLOUD_DIR_CODE, payload);
            }
            callback();
        }
    }
}
