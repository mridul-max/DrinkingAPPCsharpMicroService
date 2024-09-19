using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users.Model.DTO
{
    public class CareGiverPatient
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool Active { get; set; }
        public int DailyLimit { get; set; }
        public UserRole UserRole { get; set; }
        public int DailyGoal { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
