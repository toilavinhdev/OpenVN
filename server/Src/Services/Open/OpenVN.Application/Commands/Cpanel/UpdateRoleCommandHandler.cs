using MediatR;
using Microsoft.Extensions.Localization;
using SharedKernel.Properties;
using SharedKernel.Runtime.Exceptions;
using SharedKernel.SignalR;
using static SharedKernel.Application.Enum;

namespace OpenVN.Application
{
    public class UpdateRoleCommandHandler : BaseCommandHandler, IRequestHandler<UpdateRoleCommand, Unit>
    {
        private readonly ICpanelReadOnlyRepository _cpanelReadOnlyRepository;
        private readonly ICpanelWriteOnlyRepository _cpanelWriteOnlyRepository;
        private readonly IOpenMessageHub _hub;
        private readonly IStringLocalizer<Resources> _localizer;

        public UpdateRoleCommandHandler(
            IEventDispatcher eventDispatcher,
            IAuthService authService,
            ICpanelReadOnlyRepository cpanelReadOnlyRepository,
            ICpanelWriteOnlyRepository cpanelWriteOnlyRepository,
            IOpenMessageHub hub,
            IStringLocalizer<Resources> localizer
        ) : base(eventDispatcher, authService)
        {
            _cpanelReadOnlyRepository = cpanelReadOnlyRepository;
            _cpanelWriteOnlyRepository = cpanelWriteOnlyRepository;
            _hub = hub;
            _localizer = localizer;
        }

        public async Task<Unit> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            if (!long.TryParse(request.Dto.RoleId, out var roleId) || !long.TryParse(request.Dto.ActionId, out var actionId))
            {
                throw new BadRequestException(_localizer["bad_data"]);
            }

            var actions = await _cpanelReadOnlyRepository.GetActionsByExponentsAsync(new List<ActionExponent> { ActionExponent.SA, ActionExponent.Admin }, cancellationToken);
            if (actions.Select(x => x.Id).Contains(actionId))
            {
                throw new BadRequestException(_localizer["bad_data"]);
            }

            await _cpanelWriteOnlyRepository.UpdateRoleAsync(request.Dto.RoleId, request.Dto.ActionId, request.Dto.Value, cancellationToken);
            await _cpanelWriteOnlyRepository.UnitOfWork.CommitAsync(true, cancellationToken);

            if (!((long)RoleId.SA).ToString().Equals(request.Dto.RoleId))
            {
                var users = await _cpanelReadOnlyRepository.GetUsersByRoleId(request.Dto.RoleId, cancellationToken);
                if (users.Any())
                {
                    var keys = users.Select(x => $"{x.TenantId}_{x.Id}").ToList();
                    await _hub.SendMessages(new NotificationMessageDto { Keys = keys, Type = MessageHubType.UpdateRole, Description = "Cập nhật vai trò, xóa token" }, cancellationToken);
                }
            }
            return Unit.Value;
        }
    }
}
