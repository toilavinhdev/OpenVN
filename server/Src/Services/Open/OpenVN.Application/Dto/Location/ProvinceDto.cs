namespace OpenVN.Application
{
    public class ProvinceDto
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public LocationType Type { get; set; }

        public string Slug { get; set; }

        public int ChildrenCount { get; set; }

        public List<DistrictDto> Children { get; set; } = new();
    }
}
