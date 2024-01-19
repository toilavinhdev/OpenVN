using AutoMapper;
using Microsoft.Extensions.Localization;
using OpenVN.Application.Properties;
using SharedKernel.Auth;
using SharedKernel.Libraries;
using SharedKernel.Runtime.Exceptions;
using static SharedKernel.Application.Enum;
using Directory = OpenVN.Domain.Directory;

namespace OpenVN.Application
{
    public class AddDirectoryCommandHandler : BaseCommandHandler, IRequestHandler<AddDirectoryCommand, DirectoryDto>
    {
        private readonly IDirectoryReadOnlyRepository _directoryReadOnlyRepository;
        private readonly IDirectoryWriteOnlyRepository _directoryWriteOnlyRepository;
        private readonly ILockDirectoryService _lockDirectoryService;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<Resources> _localizer;
        private readonly IToken _token;

        public AddDirectoryCommandHandler(
            IEventDispatcher eventDispatcher,
            IAuthService authService,
            IDirectoryReadOnlyRepository directoryReadOnlyRepository,
            IDirectoryWriteOnlyRepository directoryWriteOnlyRepository,
            ILockDirectoryService lockDirectoryService,
            IMapper mapper,
            IStringLocalizer<Resources> localizer,
            IToken token
        ) : base(eventDispatcher, authService)
        {
            _directoryReadOnlyRepository = directoryReadOnlyRepository;
            _directoryWriteOnlyRepository = directoryWriteOnlyRepository;
            _lockDirectoryService = lockDirectoryService;
            _mapper = mapper;
            _localizer = localizer;
            _token = token;
        }

        public async Task<DirectoryDto> Handle(AddDirectoryCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Directory>(request.DirectoryDto);
            if (entity.ParentId > 0)
            {
                var parent = await _directoryReadOnlyRepository.GetByIdAsync<Directory>(entity.ParentId.ToString(), cancellationToken);
                if (parent == null)
                {
                    throw new BadRequestException(_localizer["directory_root_dir_does_not_exist"]);
                }

                await _lockDirectoryService.MakeSureLockedDirectoryIsSafeAsync(entity.ParentId, cancellationToken: cancellationToken);
                entity.Path = string.IsNullOrEmpty(parent.Path) ? entity.ParentId.ToString() : parent.Path + "-" + entity.ParentId;
            }

            var maxDuplicateNo = (await _directoryReadOnlyRepository.GetMaxDirectoryDuplicateNoAsync(entity, cancellationToken));
            entity.DuplicateNo = maxDuplicateNo + 1;
            entity.Name = entity.Name.ToBase64Encode();

            await _directoryWriteOnlyRepository.SaveAsync(entity, cancellationToken);
            await _directoryWriteOnlyRepository.UnitOfWork.CommitAsync(false, cancellationToken: cancellationToken);

            var clone = (Directory)entity.Clone();
            clone.ClearDomainEvents();
            clone.Name = clone.Name.ToBase64Decode();

            _ = _eventDispatcher.FireEvent(new InsertAuditEvent<Directory>(new List<Directory> { clone }, _token));

            return _mapper.Map<DirectoryDto>(entity);
        }
    }
}
