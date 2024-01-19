namespace OpenVN.Application
{
    public class DistrictDto
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public LocationType Type { get; set; }

        public string ProvinceId { get; set; }

        public int ChildrenCount { get; set; }

        public List<WardDto> Children { get; set; } = new();
    }
}
