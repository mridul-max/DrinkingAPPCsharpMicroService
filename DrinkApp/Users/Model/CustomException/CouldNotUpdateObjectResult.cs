using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users.Model.CustomException
{
    internal class CouldNotUpdateObjectResult : ObjectResult
    {
        public CouldNotUpdateObjectResult(string message) : base(message) { }
    }
}
