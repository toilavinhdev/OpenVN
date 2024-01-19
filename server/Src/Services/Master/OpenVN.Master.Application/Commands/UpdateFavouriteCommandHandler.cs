using OpenVN.Master.Application.Repositories;
using OpenVN.Master.Domain.Events;
using SharedKernel.Auth;
using SharedKernel.Runtime.Exceptions;

namespace OpenVN.Master.Application.Commands
{
    public class UpdateFavouriteCommandHandler : IRequestHandler<UpdateFavouriteCommand, Unit>
    {
        private readonly IAppWriteOnlyRepository _appWriteOnlyRepository;
        private readonly IAppReadOnlyRepository _appReadOnlyRepository;
        private readonly IEventDispatcher _eventDispatcher;
        private readonly IToken _token;

        public UpdateFavouriteCommandHandler(
            IAppWriteOnlyRepository appWriteOnlyRepository,
            IAppReadOnlyRepository appReadOnlyRepository,
            IEventDispatcher eventDispatcher,
            IToken token
        )
        {
            _appWriteOnlyRepository = appWriteOnlyRepository;
            _appReadOnlyRepository = appReadOnlyRepository;
            _eventDispatcher = eventDispatcher;
            _token = token;
        }

        public async Task<Unit> Handle(UpdateFavouriteCommand request, CancellationToken cancellationToken)
        {
            if (!long.TryParse(request.AppId, out var appId))
            {
                throw new BadRequestException("AppId is not valid");
            }
            var app = await _appReadOnlyRepository.GetByIdAsync<App>(appId, cancellationToken);
            if (app == null)
            {
                throw new BadRequestException("AppId is not valid");
            }
            if (!app.IsRelease)
            {
                throw new BadRequestException("Unreleased app");
            }

            await _appWriteOnlyRepository.UpdateFavouriteAsync(appId, request.IsFavourite, cancellationToken);
            await _appWriteOnlyRepository.UnitOfWork.CommitAsync(false, cancellationToken);

            _ = _eventDispatcher.FireEvent(new UpdateFavouriteAuditEvent(app.AppName, request.IsFavourite, _token));
            return Unit.Value;
        }
    }
}
