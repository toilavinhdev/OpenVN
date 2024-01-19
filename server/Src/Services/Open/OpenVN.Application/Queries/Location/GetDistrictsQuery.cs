namespace OpenVN.Application
{
    public class GetDistrictsQuery : BaseAllowAnonymousQuery<List<DistrictDto>>
    {
        public string ProvinceId { get; }

        public GetDistrictsQuery(string provinceId)
        {
            ProvinceId = provinceId;
        }
    }
}
