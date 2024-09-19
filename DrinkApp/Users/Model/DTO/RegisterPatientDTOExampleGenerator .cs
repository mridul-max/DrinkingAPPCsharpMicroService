using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json.Serialization;
using System;
using Users.Model;
using Users.Model.DTO;

namespace Users.Model.DTO
{
    public class RegisterPatientDTOExampleGenerator : OpenApiExample<RegisterPatientDTO>
    {
        public override IOpenApiExample<RegisterPatientDTO> Build(NamingStrategy NamingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve("ds", new RegisterPatientDTO()
            {
                FirstName = "ds",
                LastName = "Narvekar",
                PhoneNumber = "0612345678",
                Email = "abi@gmail.com",
                Password = "abc",
     /*           UserRole = new UserRole
                {
                    Role = Role.PATIENT
                },*/
                DateOfBirth = new DateTime(1950, 07, 06),
                CareGiver = new CareGiver
                {

                },
                CareGiverId = Guid.NewGuid()

            }, NamingStrategy)) ;
            return this;
        }
    }
}
