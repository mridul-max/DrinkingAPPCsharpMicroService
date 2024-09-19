using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users.Model.CustomException
{
    public class UpdateFailedException : Exception
    {
        public UpdateFailedException(string message) : base(message) { }
    }
}
