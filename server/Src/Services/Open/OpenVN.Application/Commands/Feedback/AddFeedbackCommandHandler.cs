using AutoMapper;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SharedKernel.Auth;
using SharedKernel.Libraries;
using SharedKernel.Properties;
using SharedKernel.SignalR;

namespace OpenVN.Application
{
    public class AddFeedbackCommandHandler : BaseCommandHandler, IRequestHandler<AddFeedbackCommand, Unit>
    {
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<Resources> _localizer;
        private readonly IFeedbackReadOnlyRepository _feedbackReadOnlyRepository;
        private readonly IFeedbackWriteOnlyRepository _feedbackWriteOnlyRepository;
        private readonly IOpenMessageHub _hub;
        private readonly IToken _token;
        private readonly IUserService _userService;

        public AddFeedbackCommandHandler(
            IEventDispatcher eventDispatcher,
            IAuthService authService,
            IMapper mapper,
            IStringLocalizer<Resources> localizer,
            IFeedbackReadOnlyRepository feedbackReadOnlyRepository,
            IFeedbackWriteOnlyRepository feedbackWriteOnlyRepository,
            IOpenMessageHub hub,
            IToken token,
            IUserService userService
        ) : base(eventDispatcher, authService)
        {
            _mapper = mapper;
            _localizer = localizer;
            _feedbackReadOnlyRepository = feedbackReadOnlyRepository;
            _feedbackWriteOnlyRepository = feedbackWriteOnlyRepository;
            _hub = hub;
            _token = token;
            _userService = userService;
        }

        public async Task<Unit> Handle(AddFeedbackCommand request, CancellationToken cancellationToken)
        {
            request.Dto.FromIP = AuthUtility.TryGetIP(_token.Context.HttpContext.Request);
            var entity = _mapper.Map<Feedback>(request.Dto);
            if (entity.ParentId > 0)
            {
                var parent = await _feedbackReadOnlyRepository.GetByIdAsync<Feedback>(entity.ParentId, cancellationToken);
                if (parent == null)
                {
                    throw new BadRequestException(_localizer["repository_data_does_not_exist_or_was_deleted"]);
                }

                if (!string.IsNullOrEmpty(parent.Path))
                {
                    entity.Path = parent.Path + "/" + parent.Id;
                }
                else
                {
                    entity.Path = parent.Id.ToString();
                }
            }

            await _feedbackWriteOnlyRepository.SaveAsync(entity, cancellationToken);
            await _feedbackWriteOnlyRepository.UnitOfWork.CommitAsync(false, cancellationToken);

            var feedback = await _feedbackReadOnlyRepository.GetByIdAsync<FeedbackDto>(entity.Id, cancellationToken);
            if (!string.IsNullOrEmpty(feedback.AvatarUrl))
            {
                feedback.AvatarUrl = await _userService.GetAvatarUrlByFileNameAsync(feedback.AvatarUrl, entity.TenantId, entity.OwnerId, cancellationToken);
            }

            await _hub.SendMessages(new NotificationMessageDto
            {
                IsAllClients = true,
                Type = MessageHubType.NewFeedback,
                Description = JsonConvert.SerializeObject(feedback, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                })
            });

            return Unit.Value;
        }
    }
}
