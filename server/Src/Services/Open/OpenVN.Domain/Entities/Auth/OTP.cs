using static SharedKernel.Application.Enum;

namespace OpenVN.Domain
{
    [Table("auth_otp")]
    public class OTP
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public string Otp { get; set; }

        public bool IsUsed { get; set; }

        public DateTime ExpriedDate { get; set; }

        public DateTime ProvidedDate { get; set; }

        public OtpType Type { get; set; } = OtpType.None;
    }
}
