using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Diagnostics.CodeAnalysis;

namespace Users.Model
{
    public class CareGiver : IEntityBase
    {
        [Key]
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public List<UserRole> UserRoles { get; set; }
        public bool Active { get; set; }
        //[JsonIgnore]
        //[IgnoreDataMember]
        //[AllowNull]
#nullable enable
        public virtual List<Patient>? Patients { get; set; } = new List<Patient>();
#nullable disable
        [AllowNull]
        public string GenerateTokenCode { get; set; }
        public DateTime TokenCodeGeneratedTime { get; set; }
    }
}
