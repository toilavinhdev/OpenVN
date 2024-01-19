using SharedKernel.Domain;

namespace OpenVN.BackgroundJob
{
    public class SignInBody
    {
        public User TokenUser { get; set; }

        public string RequestId { get; set; }
    }
}
