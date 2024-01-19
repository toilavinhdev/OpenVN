using SharedKernel.Domain;
using SharedKernel.Libraries;
using SharedKernel.Log;

namespace OpenVN.BackgroundJob
{
    public class IntegrationAuthNoticeService : IIntegrationAuthNoticeService
    {
        public async Task SignInWarningAsync(User user, RequestValue request, DateTime timestamp, CancellationToken cancellationToken = default)
        {
            try
            {
                var json = JsonHelper.GetConfiguration("notification-template.json");
                var subject = json.GetSection("SignInWarning:Subject").Value;
                var template = json.GetSection("SignInWarning:Template").Value;
                var body = string.Format(
                    template,
                    request.OSFamily,
                    user.FirstName,
                    user.Username,
                    request.OSFamily,
                    timestamp.DateFullText(),
                    request.Ip,
                    request.Device,
                    request.Browser,
                    request.OS,
                    "https://kibana.vn"
                );
                var option = new EmailOptionRequest()
                {
                    DisplayName = "OpenVN Security",
                    To = user.Email,
                    Subject = string.Format(subject, user.Email),
                    Body = body
                };

                await Task.Yield();
                EmailHelper.SendMail(option);
            }
            catch (Exception ex)
            {
                Logging.Error(ex);
                throw;
            }
        }

    }
}
