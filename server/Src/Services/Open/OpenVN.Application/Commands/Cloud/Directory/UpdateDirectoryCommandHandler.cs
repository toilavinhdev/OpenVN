using AutoMapper;
using Microsoft.Extensions.Localization;
using OpenVN.Application.Properties;
using SharedKernel.Libraries;
using SharedKernel.Runtime.Exceptions;
using static SharedKernel.Application.Enum;
using Directory = OpenVN.Domain.Directory;

namespace OpenVN.Application
{
    public class UpdateDirectoryCommandHandler : BaseCommandHandler, IRequestHandler<UpdateDirectoryCommand, Unit>
    {
        private readonly IDirectoryReadOnlyRepository _directoryReadOnlyRepository;
        private readonly IDirectoryWriteOnlyRepository _directoryWriteOnlyRepository;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<Resources> _localizer;

        public UpdateDirectoryCommandHandler(IEventDispatcher eventDispatcher, IAuthService authService, IDirectoryReadOnlyRepository directoryReadOnlyRepository, IDirectoryWriteOnlyRepository directoryWriteOnlyRepository, IMapper mapper, IStringLocalizer<Resources> localizer) : base(eventDispatcher, authService)
        {
            _directoryReadOnlyRepository = directoryReadOnlyRepository;
            _directoryWriteOnlyRepository = directoryWriteOnlyRepository;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<Unit> Handle(UpdateDirectoryCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Directory>(request.DirectoryDto);
            if (entity.ParentId > 0)
            {
                var parent = await _directoryReadOnlyRepository.GetByIdAsync<Directory>(entity.ParentId.ToString(), cancellationToken);
                if (parent == null)
                {
                    throw new BadRequestException(_localizer["directory_root_dir_does_not_exist"]);
                }
                entity.Path = string.IsNullOrEmpty(parent.Path) ? entity.ParentId.ToString() : parent.Path + "-" + entity.ParentId;
            }

            var maxDuplicateNo = (await _directoryReadOnlyRepository.GetMaxDirectoryDuplicateNoAsync(entity, cancellationToken));
            entity.DuplicateNo =  maxDuplicateNo + 1;
            entity.Name = entity.Name.ToBase64Encode();

            await _directoryWriteOnlyRepository.UpdateAsync(entity, cancellationToken: cancellationToken);
            await _directoryWriteOnlyRepository.UnitOfWork.CommitAsync(cancellationToken: cancellationToken);

            return Unit.Value;
        }
    }
}
