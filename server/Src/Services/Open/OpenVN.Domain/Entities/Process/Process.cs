﻿namespace OpenVN.Domain
{
    [Table("process")]
    public class Process : PersonalizedEntity
    {
        public string Message { get; set; }

        public bool Enabled { get; set; }

        public DateTime LastNotificationTime { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int Period { get; set; }

        public double Percent { get; set; }

        public int Consecutiveness { get; set; }

        public bool IsRepeat { get; set; }
    }
}
