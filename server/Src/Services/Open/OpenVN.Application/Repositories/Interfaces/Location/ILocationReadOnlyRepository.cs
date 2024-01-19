namespace OpenVN.Application
{
    public interface ILocationReadOnlyRepository : IBaseReadOnlyRepository<BaseLocation>
    {
        Task<IEnumerable<ProvinceDto>> GetProvincesAsync(CancellationToken cancellationToken);

        Task<IEnumerable<DistrictDto>> GetDistrictsAsync(string provinceId, CancellationToken cancellationToken);

        Task<IEnumerable<WardDto>> GetWardsAsync(string districtId, CancellationToken cancellationToken);

        Task<IEnumerable<ProvinceDto>> SearchLocationsAsync(string query, CancellationToken cancellationToken);

        Task<PagingResult<RankDto>> GetRankPagingAsync(PagingRequest request, CancellationToken cancellationToken);
    }
}
