using Microsoft.Extensions.Localization;
using OpenVN.Application.Properties;
using SharedKernel.Auth;
using SharedKernel.Runtime.Exceptions;

namespace OpenVN.Application
{
    public class DeleteNoteCommandHandler : BaseCommandHandler, IRequestHandler<DeleteNoteCommand, Unit>
    {
        private readonly INoteWriteOnlyRepository _noteWriteOnlyRepository;
        private readonly INoteReadOnlyRepository _noteReadOnlyRepository;
        private readonly IStringLocalizer<Resources> _localizer;
        private readonly IToken _token;

        public DeleteNoteCommandHandler(
            IEventDispatcher eventDispatcher, 
            IAuthService authService, 
            INoteWriteOnlyRepository noteWriteOnlyRepository, 
            INoteReadOnlyRepository noteReadOnlyRepository, 
            IStringLocalizer<Resources> localizer,
            IToken token
        ) : base(eventDispatcher, authService)
        {
            _noteWriteOnlyRepository = noteWriteOnlyRepository;
            _noteReadOnlyRepository = noteReadOnlyRepository;
            _localizer = localizer;
            _token = token;
        }

        public async Task<Unit> Handle(DeleteNoteCommand request, CancellationToken cancellationToken)
        {
            foreach (var id in request.Ids)
            {
                var entity = await _noteReadOnlyRepository.GetByIdAsync<Note>(id, cancellationToken)
                             ?? throw new BadRequestException(_localizer["common_data_does_not_exist_or_was_deleted"].Value);
                await _noteWriteOnlyRepository.UpdateFromIndexOrderToLastAsync(entity.Order, 0, entity.CategoryId, false, cancellationToken);
            }

            await _noteWriteOnlyRepository.DeleteAsync(request.Ids, cancellationToken);
            await _noteWriteOnlyRepository.UnitOfWork.CommitAsync(cancellationToken: cancellationToken);

            return Unit.Value;
        }
    }
}
