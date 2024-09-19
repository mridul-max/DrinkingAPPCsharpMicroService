using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users.Model.CustomException
{
    public class EntitiesNotFoundException : Exception
    {
        public EntitiesNotFoundException(string message) : base(message) { }
    }
}
