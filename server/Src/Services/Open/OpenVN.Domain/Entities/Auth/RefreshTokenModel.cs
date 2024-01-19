namespace OpenVN.Domain
{
    [Table("auth_refresh_token")]
    public class RefreshToken : PersonalizedEntity
    {
        public string RefreshTokenValue { get; set; }

        public string CurrentAccessToken { get; set; }

        public DateTime ExpriedDate { get; set; }
    }
}
