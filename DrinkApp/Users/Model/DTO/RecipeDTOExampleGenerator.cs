/*using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Resolvers;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users.Model.DTO
{
    public class RecipeDTOExampleGenerator : OpenApiExample<RecipeDTO>
    {
        public override IOpenApiExample<RecipeDTO> Build(NamingStrategy NamingStrategy = null)
        {
            Examples.Add(OpenApiExampleResolver.Resolve("Recipe", new RecipeDTO()
            {
                RecipeId = Guid.NewGuid(),
            }, NamingStrategy));
            return this;
        }
    }
}
*/