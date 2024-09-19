using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Data;
using Users.Model;
using Users.Model.DTO;

namespace Users.Model.DTO
{
    public class RegisterCareGiverDTOExampleGenerator : OpenApiExample<RegisterCareGiverDTO>
    {
        public override IOpenApiExample<RegisterCareGiverDTO> Build(NamingStrategy NamingStrategy = null)
        {
            Patient p = new Patient()
            {
                FirstName = "zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz",
                LastName = "Narvekar",
                PhoneNumber = "+31645876734",
                Active = false,
                DailyLimit = 45,
                UserRole = new UserRole { Role = Role.PATIENT },
                DailyGoal = 49,
                DateOfBirth = new DateTime(1950, 12, 12)
            };
            Patient t = new Patient()
            {
                FirstName = "tttttttttttttttttttttttttttttttt",
                LastName = "Narvekar",
                PhoneNumber = "+31645876734",
                Active = false,
                DailyLimit = 45,
                UserRole = new UserRole { Role = Role.PATIENT },
                DailyGoal = 49,
                DateOfBirth = new DateTime(1950, 12, 12)
            };

            Examples.Add(OpenApiExampleResolver.Resolve("Abhishek", new RegisterCareGiverDTO()
            {
                FirstName = "Abhishek",
                LastName = "Narvekar",
                PhoneNumber = "0612345678",
                Email = "abi@gmail.com",
                Password = "abc",
               
                
            }, NamingStrategy));
            return this;
        }
    }
}
