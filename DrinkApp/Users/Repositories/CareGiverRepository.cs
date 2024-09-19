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
    public class CareGiverRepository : EntityBaseRepository<CareGiver>, ICareGiverRepository
    {
        public CareGiverRepository(UserDbContext applicationDbContext) : base(applicationDbContext)
        {

        }
    }
}
