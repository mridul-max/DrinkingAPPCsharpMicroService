using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Users.Model.CustomException
{
    public class UserCreationConflictObjectResult : ObjectResult
    {
        public UserCreationConflictObjectResult(string message) : base(message) { }
    }
}
