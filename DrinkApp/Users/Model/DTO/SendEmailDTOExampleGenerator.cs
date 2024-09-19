using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json.Serialization;

namespace Users.Model.DTO
{
    public class SendEmailDTOExampleGenerator : OpenApiExample<SendEmailDTO>
    {
        public override IOpenApiExample<SendEmailDTO> Build(NamingStrategy NamingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve("Abhishek", new SendEmailDTO()
            {
                Email = "mahedimridul57@gmail.com"
            }, NamingStrategy)); ;
            return this;
        }
    }
}
