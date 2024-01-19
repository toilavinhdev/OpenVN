using Microsoft.Extensions.Localization;
using OpenVN.Application.Properties;
using SharedKernel.Auth;
using SharedKernel.Caching;
using SharedKernel.Libraries;
using SharedKernel.Providers;
using SharedKernel.Runtime.Exceptions;

namespace OpenVN.Application
{
    public class SetAvatarCommandHandler : BaseCommandHandler, IRequestHandler<SetAvatarCommand, Unit>
    {
        private readonly IUserWriteOnlyRepository _userWriteOnlyRepository;
        private readonly IStringLocalizer<Resources> _localizer;
        private readonly IS3StorageProvider _s3;
        private readonly IToken _token;
        private readonly ISequenceCaching _caching;

        public SetAvatarCommandHandler(
            IEventDispatcher eventDispatcher,
            IAuthService authService,
            IUserWriteOnlyRepository userWriteOnlyRepository,
            IStringLocalizer<Resources> localizer,
            IS3StorageProvider s3,
            IToken token,
            ISequenceCaching caching
        ) : base(eventDispatcher, authService)
        {
            _userWriteOnlyRepository = userWriteOnlyRepository;
            _localizer = localizer;
            _s3 = s3;
            _token = token;
            _caching = caching;
        }

        public async Task<Unit> Handle(SetAvatarCommand request, CancellationToken cancellationToken)
        {
            if (request.Avatar == null || !FileHelper.IsImage(request.Avatar))
            {
                throw new BadRequestException(_localizer["common_payload_is_not_valid"]);
            }

            // Đẩy files lên s3
            var uploadRequest = new UploadRequest
            {
                FileName = request.Avatar.FileName,
                FileExtension = Path.GetExtension(request.Avatar.FileName).ToLower(),
                Size = request.Avatar.Length,
                Stream = request.Avatar.OpenReadStream(),
            };

            var s3Response = await _s3.UploadAsync(uploadRequest, cancellationToken);
            if (!s3Response.Success)
            {
                throw new CatchableException(_localizer["user_upload_avatar_failed"]);
            }

            await _userWriteOnlyRepository.SetAvatarAsync(s3Response.CurrentFileName, cancellationToken);
            await _userWriteOnlyRepository.UnitOfWork.CommitAsync(true, cancellationToken);

            _ = _eventDispatcher.FireEvent(new UpdateAvatarAuditEvent(_token));

            var cacheKey = OpenCacheKeys.GetAvatarUrlKey(_token.Context.TenantId, _token.Context.OwnerId);
            await _caching.RemoveAsync(cacheKey, cancellationToken: cancellationToken);

            return Unit.Value;
        }
    }
}
