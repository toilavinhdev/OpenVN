namespace OpenVN.Domain
{
    public class AccessNote : BaseEntity
    {
        public long NoteId { get; set; }

        public string Status { get; set; }

        public string Ip { get; set; }

        public string Browser { get; set; }

        public string OS { get; set; }

        public string Device { get; set; }

        public string UA { get; set; }

        public string MAC { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public string Lat { get; set; }

        public string Long { get; set; }

        public string Timezone { get; set; }

        public string Org { get; set; }

        public string Postal { get; set; }

        public string Origin { get; set; }
    }
}
