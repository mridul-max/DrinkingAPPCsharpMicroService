﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users.Model.CustomException
{
    public class SuccessMessageResponse : ObjectResult
    {
        public SuccessMessageResponse(object value) : base(value)
        {
        }
    }
}
