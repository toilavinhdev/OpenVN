using System.ComponentModel.DataAnnotations.Schema;

namespace SharedKernel.Domain
{
    [Table("common_user_config")]
    public class UserConfig : PersonalizedEntity
    {
        public string Json { get; set; }
    }
}
