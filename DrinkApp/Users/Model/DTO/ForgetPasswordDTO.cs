using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users.Model.DTO
{
    public class ForgetPasswordDTO
    {
        [JsonRequired]
        public string GenerateTokenCode { get; set; }
        [JsonRequired]
        public string NewPassword { get; set; }
    }
}
