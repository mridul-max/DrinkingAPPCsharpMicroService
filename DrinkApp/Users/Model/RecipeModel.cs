/*using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users.Model
{
    public class RecipeModel
    {
        [Key]
        public Guid RecipeId { get; set; }
        public string Name { get; set; }
        public string Discriminator { get; set; }
        public Dictionary<string, string> Ingredients { get; set; }
        public string Instructions { get; set; }
        public bool Visible { get; set; }
        public List<Patient> patients { get; set; }
        public List<PatientRecipe> patientRecipes { get; set; }
    }
}
*/