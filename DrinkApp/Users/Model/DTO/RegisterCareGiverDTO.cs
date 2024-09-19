using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Users.Model;

namespace Users.Model.DTO
{
    public class RegisterCareGiverDTO
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
        public List<Patient> Patients { get; set; } 
    }
}
