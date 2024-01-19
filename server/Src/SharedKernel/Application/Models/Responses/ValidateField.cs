using static SharedKernel.Application.Enum;

namespace SharedKernel.Application
{
    public class ValidateField
    {
        public string FieldName { get; set; }

        public ValidateCode Code { get; set; }

        public string ErrorMessage { get; set; }
    }
}
