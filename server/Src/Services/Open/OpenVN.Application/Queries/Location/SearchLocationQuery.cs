namespace OpenVN.Application
{
    public class SearchLocationQuery : BaseAllowAnonymousQuery<List<ProvinceDto>>
    {
        public string Query { get; }

        public SearchLocationQuery(string query)
        {
            Query = query;
        }
    }
}
