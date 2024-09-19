using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Users.Model;

namespace Users.AppCrudRepositories
{
    public interface IGenericRepository<T> where T : class, IEntityBase, new()
    {
        IEnumerable<T> GetAll();
        Task<T> GetByIdAsync(Guid id);
        Task<T> GetByPhoneNumber(string phoneNumber);
        void Insert(T obj);
        void Update(T obj);
        void Delete(object id);
        void Save();
    }
}
