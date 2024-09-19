using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users.Model.DTO
{
    public class ForgetPasswordDTOExampleGenerator : OpenApiExample<ForgetPasswordDTO>
    {
        public override IOpenApiExample<ForgetPasswordDTO> Build(NamingStrategy NamingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve("Abhishek", new ForgetPasswordDTO()
            {
                GenerateTokenCode = "wer",
                NewPassword = "sheikh"
            }, NamingStrategy));
            return this;
        }
    }
}