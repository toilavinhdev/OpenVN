﻿using System.ComponentModel.DataAnnotations.Schema;
using static SharedKernel.Application.Enum;

namespace SharedKernel.Domain
{
    [Table("common_user")]
    public class User : BaseEntity
    {
        public string Username { get; set; }

        public string PasswordHash { get; set; }

        public string Salt { get; set; }

        public string PhoneNumber { get; set; }

        public bool ConfirmedPhone { get; set; }

        public string Email { get; set; }

        public bool ConfirmedEmail { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; }

        public DateTime DateOfBirth { get; set; }

        public GenderType Gender { get; set; }

        public long PositionId { get; set; }

        public long DepartmentId { get; set; }
    }
}
