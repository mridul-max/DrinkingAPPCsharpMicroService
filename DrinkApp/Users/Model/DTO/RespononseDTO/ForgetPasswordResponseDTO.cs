using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users.Model.DTO.RespononseDTO
{
    public class ForgetPasswordResponseDTO
    {
        public string Message { get; set; }
        public ForgetPasswordResponseDTO(string Message)
        {
            this.Message = Message;
        }
        public override string ToString()
        {
            return Message;
        }
    }
}
