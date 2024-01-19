namespace OpenVN.Application
{
    public class GetWardsQuery : BaseAllowAnonymousQuery<List<WardDto>>
    {
        public string DistrictId { get; }

        public GetWardsQuery(string districtId)
        {
            DistrictId = districtId;
        }
    }
}
