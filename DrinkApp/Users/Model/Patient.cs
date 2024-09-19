
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Users.Model
{
    public class Patient : IEntityBase
    {
        [Key]
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public bool Active { get; set; }
#nullable enable
        public virtual CareGiver? CareGiver { get; set; } = new();
#nullable disable
        public int DailyLimit { get; set; }

        public UserRole UserRole { get; set; }    

        public int DailyGoal { get; set; }
        public DateTime DateOfBirth { get; set; }
        [AllowNull]
        public string GenerateTokenCode { get; set; }  
        public DateTime TokenCodeGeneratedTime { get; set; }
    }
}
