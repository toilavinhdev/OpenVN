namespace OpenVN.Application
{
    public class ProcessDto
    {
        public string Id { get; set; }

        public string Message { get; set; }

        public bool Enabled { get; set; } = true;

        public DateTime LastNotificationTime { get; set; }

        public int Period { get; set; }

        public double Percent { get; set; }

        public int Consecutiveness { get; set; }

        public bool IsRepeat { get; set; }
    }

    public class ProcessDtoValidator : AbstractValidator<ProcessDto>
    {
        public ProcessDtoValidator()
        {
            RuleFor(x => x.Message).NotEmpty().WithMessage("Message cannot be null or empty");
            RuleFor(x => x.Period).Must(p => p >= 30).WithMessage("Period must be greater or equal 30 minutes");
        }
    }
}
