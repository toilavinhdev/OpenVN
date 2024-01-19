using SharedKernel.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpenVN.Master.Domain
{
    [Table("app")]
    public class App : BaseEntity
    {
        public string AppName { get; set; }

        public string Host { get; set; }

        public string RedirectUrl { get; set; }

        public string IconUrl { get; set; }

        public bool IsRelease { get; set; }

        public bool IsShow { get; set; }

        public bool RequiredAuth { get; set; }

        public bool RequiredLicense { get; set; }
     
        public int Order { get; set; }
    }
}
