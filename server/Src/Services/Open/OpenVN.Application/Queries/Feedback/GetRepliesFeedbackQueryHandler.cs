using AutoMapper;
using Microsoft.Extensions.Localization;
using SharedKernel.Properties;

namespace OpenVN.Application
{
    public class GetRepliesFeedbackQueryHandler : BaseQueryHandler, IRequestHandler<GetRepliesFeedbackQuery, List<FeedbackDto>>
    {
        private readonly IStringLocalizer<Resources> _localizer;
        private readonly IFeedbackReadOnlyRepository _feedbackReadOnlyRepository;

        public GetRepliesFeedbackQueryHandler(
            IAuthService authService,
            IMapper mapper,
            IStringLocalizer<Resources> localizer,
            IFeedbackReadOnlyRepository feedbackReadOnlyRepository
        ) : base(authService, mapper)
        {
            _localizer = localizer;
            _feedbackReadOnlyRepository = feedbackReadOnlyRepository;
        }

        public async Task<List<FeedbackDto>> Handle(GetRepliesFeedbackQuery request, CancellationToken cancellationToken)
        {
            if (!long.TryParse(request.Id, out var id))
            {
                throw new BadRequestException(_localizer["bad_data"]);
            }
            var feedbacks = await _feedbackReadOnlyRepository.GetRepliesAsync<FeedbackDto>(id, cancellationToken);
            return feedbacks.ToList();
        }
    }
}
