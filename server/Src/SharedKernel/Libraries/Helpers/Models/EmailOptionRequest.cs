using SharedKernel.Core;
using System.Net.Mail;
using System.Web;

namespace SharedKernel.Libraries
{
    public class EmailOptionRequest
    {
        public EmailOptionRequest()
        {
        }

        public EmailOptionRequest(string to, string subject, string body)
        {
            To = to;
            Subject = subject;
            Body = body;
        }

        public EmailOptionRequest(string to, string subject, string body, string displayName)
        {
            To = to;
            Subject = subject;
            Body = body;
            DisplayName = displayName;
        }

        public EmailOptionRequest(string sender, string to, string subject, string body, string displayName)
        {
            _sender = sender;
            To = to;
            Subject = subject;
            Body = body;
            DisplayName = displayName;
        }

        private string _sender;

        private string _displayName;

        public string Sender
        {
            get
            {
                if (string.IsNullOrEmpty(_sender))
                {
                    _sender = DefaultEmailConfig.Sender;    
                }
                return _sender;
            }
        }

        public string To { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public string DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(_displayName))
                {
                    _displayName = DefaultEmailConfig.DisplayName;
                }
                return _displayName;
            }
            set
            {
                _displayName = value;
            }
        }

        public MailPriority Priority { get; set; } = MailPriority.Normal;

        public bool IsBodyHTML => Body != HttpUtility.HtmlEncode(Body);
    }
}
