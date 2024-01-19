using AutoMapper;

namespace OpenVN.Application
{
    public class PagingFeedbackQueryHandler : BaseQueryHandler, IRequestHandler<PagingFeedbackQuery, PagingResult<FeedbackDto>>
    {
        private readonly IFeedbackReadOnlyRepository _feedbackReadOnlyRepository;

        public PagingFeedbackQueryHandler(
            IAuthService authService,
            IMapper mapper,
            IFeedbackReadOnlyRepository feedbackReadOnlyRepository
        ) : base(authService, mapper)
        {
            _feedbackReadOnlyRepository = feedbackReadOnlyRepository;
        }

        public async Task<PagingResult<FeedbackDto>> Handle(PagingFeedbackQuery request, CancellationToken cancellationToken)
        {
            return await _feedbackReadOnlyRepository.GetPagingAsync<FeedbackDto>(request.Request, cancellationToken);
        }
    }
}
