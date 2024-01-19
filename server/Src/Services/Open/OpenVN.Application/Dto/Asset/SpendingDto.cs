using SharedKernel.Libraries;

namespace OpenVN.Application
{
    public class SpendingDto
    {
        public string Id { get; set; }

        public string CategoryId { get; set; }

        public string CategoryName { get; set; }

        public SpendingStatus Status { get; set; } = SpendingStatus.Confirmed;

        public bool IsRepeat { get; set; }

        public int RepeatEachHour { get; set; }

        public string Content { get; set; }

        public decimal Value { get; set; }

        private decimal _total;

        public decimal Total
        {
            get
            {
                if (_total > 0)
                    return _total;

                if (Subs != null && Subs.Any())
                    return Value + Subs.Sum(s => s.Total);

                return Value;
            }
            set
            {
                _total = value; 
            }
        }

        public DateTime SpendDate { get; set; } = DateHelper.Now;

        public string ParentId { get; set; }

        public List<SpendingDto> Subs { get; set; }

        public DateTime CreatedDate { get; set; } = DateHelper.Now;
    }

    public class SpendingDtoValidator : AbstractValidator<SpendingDto>
    {
        public SpendingDtoValidator()
        {
            RuleFor(x => x.Content).NotEmpty().WithMessage("Reason of this spending cannot be left blank");
            RuleFor(x => x.Value).Must(v => v > 0).WithMessage("Value of this spending must be greater than 0");
            RuleFor(x => x.CategoryId).Must(t => Int64.TryParse(t, out var result) && result > 0).WithMessage("Type of this spending is not valid");
            RuleFor(x => x).Must(x => !x.IsRepeat || x.RepeatEachHour >= 1).WithMessage("Can only repeat every 1 hour");
        }
    }
}
