using Microsoft.Azure.Cosmos.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.AppCrudRepositories;
using Users.DataAccess;
using Users.Model;

namespace Users.Repositories
{
    public class PatientRepository : EntityBaseRepository<Patient>, IPatientRepository
    {
        public PatientRepository(UserDbContext applicationDbContext) : base(applicationDbContext)
        {

        }
    }
}
