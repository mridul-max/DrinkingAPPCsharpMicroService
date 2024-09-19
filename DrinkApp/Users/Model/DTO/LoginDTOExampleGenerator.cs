using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json.Serialization;

namespace Users.Model.DTO
{
    public class LoginDTOExampleGenerator : OpenApiExample<LoginDTO>
    {
        public override IOpenApiExample<LoginDTO> Build(NamingStrategy NamingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve("Abhishek", new LoginDTO()
            {
                PhoneNumber = "017910476435",
                Password = "wer",
            }, NamingStrategy));
            return this;
        }
    }
}
