namespace OpenVN.Master.Application
{
    public class AppDto
    {
        public string Id { get; set; }

        public string AppName { get; set; }

        public string Host { get; set; }

        public string RedirectUrl { get; set; }

        public string IconUrl { get; set; }

        public bool IsFavourite { get; set; }

        public bool IsRelease { get; set; }

        public bool RequiredLicense { get; set; }

        public bool RequiredAuth { get; set; }

        public bool HasLicense { get; set; }
    }
}
