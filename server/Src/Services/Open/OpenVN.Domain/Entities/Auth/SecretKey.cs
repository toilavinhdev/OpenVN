namespace OpenVN.Domain
{
    [Table("auth_secret_key")]
    public class SecretKey : PersonalizedEntity
    {
        public string Key { get; set; }
    }
}
