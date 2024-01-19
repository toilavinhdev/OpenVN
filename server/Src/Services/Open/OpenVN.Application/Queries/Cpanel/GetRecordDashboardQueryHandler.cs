using AutoMapper;
using OpenVN.Application.Queries.Cpanel;
using SharedKernel.MySQL;

namespace OpenVN.Application
{
    public class GetRecordDashboardQueryHandler : BaseQueryHandler, IRequestHandler<GetRecordDashboardQuery, List<RecordDashboardDto>>
    {
        private readonly ICpanelReadOnlyRepository _cpanelReadOnlyRepository;

        public GetRecordDashboardQueryHandler(
            IAuthService authService,
            IMapper mapper,
            ICpanelReadOnlyRepository cpanelReadOnlyRepository
        ) : base(authService, mapper)
        {
            _cpanelReadOnlyRepository = cpanelReadOnlyRepository;
        }

        public async Task<List<RecordDashboardDto>> Handle(GetRecordDashboardQuery request, CancellationToken cancellationToken)
        {
            var result = await _cpanelReadOnlyRepository.GetRecordDashboardAsync(cancellationToken);
            using (var conn = new DbConnection("CentralizedRequestsDb"))
            {
                var requests = await conn.QuerySingleOrDefaultAsync<int>($"SELECT COUNT(*) FROM {new RequestInformation().GetTableName()}");
                result.Add(new RecordDashboardDto
                {
                    Type = "request",
                    Title = "Tổng số requests",
                    Value = requests
                });
            }

            return result;
        }
    }
}
