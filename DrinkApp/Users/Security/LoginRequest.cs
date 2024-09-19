using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users.Security
{
    public class LoginRequest
    {
        [OpenApiProperty(Description = "Username for the user logging in.")]
        [JsonRequired]
        public string PhoneNumber { get; set; }

        [OpenApiProperty(Description = "Password for the user logging in.")]
        [JsonRequired]
        public string Password { get; set; }

    }
}
