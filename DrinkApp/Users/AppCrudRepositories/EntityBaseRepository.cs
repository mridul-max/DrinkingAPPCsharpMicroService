using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Users.DataAccess;
using Users.Model;

namespace Users.AppCrudRepositories
{
    public class EntityBaseRepository<T> : IGenericRepository<T> where T : class, IEntityBase, new()
    {
        private UserDbContext _context;
        private DbSet<T> table = null;
        public EntityBaseRepository(UserDbContext context)
        {
            this._context = context;
            table = _context.Set<T>();
        }
        public IEnumerable<T> GetAll()
        {
            return table.ToList();
        }
        public async Task<T> GetByIdAsync(Guid id)
        {
            return await table.FindAsync(id);
        }
        public async Task<T> GetByPhoneNumber(string phoneNumber)
        {
            return await table.Where(x => x.PhoneNumber.Equals(phoneNumber)).FirstOrDefaultAsync();
        }
        public void Insert(T obj)
        {
            table.Add(obj);
        }
        public void Update(T obj)
        {
            table.Attach(obj);
            _context.Entry(obj).State = EntityState.Modified;
            _context.SaveChanges();
        }
        public void Delete(object id)
        {
            T existing = table.Find(id);
            table.Remove(existing);
        }
        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
