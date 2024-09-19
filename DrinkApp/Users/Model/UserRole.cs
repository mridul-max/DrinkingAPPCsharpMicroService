using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users.Model
{
    public class UserRole
    {
        [Key]
        [JsonIgnore]
        public Guid RoleId { get; set; }
        public Role Role { get; set; }
    }
}
