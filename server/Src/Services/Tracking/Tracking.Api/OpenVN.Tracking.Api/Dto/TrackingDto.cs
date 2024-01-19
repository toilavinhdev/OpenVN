namespace OpenVN.TrackingApi.Dto
{
    public class TrackingDto
    {
        public long Id { get; set; }

        public string Data { get; set; }

        public string EventId { get; set; }

        public string PreviousScreen { get; set; }

        public string CurrentScreen { get; set; }

        public string Origin { get; set; }

        public int ScreenWidth { get; set; }

        public int ScreenHeight { get; set; }

        public int ScreenInnerWidth { get; set; }

        public int ScreenInnerHeight { get; set; }

        public string Language { get; set; }

        public long Time { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
