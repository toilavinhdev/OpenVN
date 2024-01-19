namespace OpenVN.Application
{
    public class PathDto
    {
        public string Path { get; set; }

        public List<PathMapping> Mapping { get; set; }
    }

    public class PathMapping
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}
