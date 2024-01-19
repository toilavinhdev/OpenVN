using SharedKernel.Application;

namespace OpenVN.Application
{
    public class AuthResponse : BaseResponse
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }
    }
}
