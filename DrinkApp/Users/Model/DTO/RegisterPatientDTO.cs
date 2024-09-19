using Newtonsoft.Json;
using System;

namespace Users.Model.DTO
{
    public class RegisterPatientDTO
    {
        [JsonRequired]
        public string FirstName { get; set; }
        [JsonRequired]
        public string LastName { get; set; }
        [JsonRequired]
        public string Email { get; set; }
        [JsonRequired]
        public string PhoneNumber { get; set; }
        [JsonRequired]
        public string Password { get; set; }
    
        //public bool Active { get; set; }
        [JsonRequired]
        public int DailyLimit { get; set; }
        [JsonIgnore]
        public CareGiver CareGiver { get; set; }

        [JsonRequired]
       // public UserRole UserRole { get; set; }
        public int DailyGoal { get; set; }

        public DateTime DateOfBirth { get; set; }
        public Guid CareGiverId { get; set; }
    }
}
