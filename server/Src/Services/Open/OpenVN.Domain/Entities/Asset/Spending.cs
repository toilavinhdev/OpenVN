namespace OpenVN.Domain
{
    public class Spending : PersonalizedEntity
    {
        public long SpendingCategoryId { get; set; }

        public SpendingStatus Status { get; set; } = SpendingStatus.Confirmed;

        public bool IsRepeat { get; set; }

        public int RepeatEachHour { get; set; }

        public string Content { get; set; }

        public decimal Value { get; set; }

        public DateTime SpendDate { get; set; }

        public long ParentId { get; set; }

        public string Path { get; set; }
    }
}
