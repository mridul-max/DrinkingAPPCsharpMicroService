using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users.Model.DTO
{
    public class CareGiverDTO
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public List<UserRole> UserRoles { get; set; }
        public bool Active { get; set; }
        public List<CareGiverPatient> Patients { get; set; } = new List<CareGiverPatient>();
        
    }
}
