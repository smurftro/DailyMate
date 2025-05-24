using Domain;
using Domain.Repository;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class WriteRepository<T> : IWriteRepository<T> where T : class,IEntityBase
    {
        private readonly AppDbContext _context;
        public WriteRepository(AppDbContext appDbContext) 
        { 
            _context = appDbContext;
        }
        public DbSet<T> Table => _context.Set<T>();

        public async Task<bool> AddAsync(T model) 
        {
           EntityEntry<T> entityEntry=await Table.AddAsync(model);
            return entityEntry.State == EntityState.Added;         
        }

        public async Task<bool> AddRangeAsync(List<T> model)
        {
            await Table.AddRangeAsync(model);
            return true;
        }

        public  bool Delete(T model)
        {
           EntityEntry<T> entityEntry=Table.Remove(model);
            return entityEntry.State==EntityState.Deleted;
        }

       public  bool Delete(Guid id)
        {
           var user=Table.FirstOrDefault(x => x.Id == id);
            if (user == null) return false;
            EntityEntry<T> entityEntry=Table.Remove(user);
            return entityEntry.State == EntityState.Deleted;
        }
        public bool DeleteRange(IEnumerable<T> models)
        {
           Table.RemoveRange(models);
           return true ;
        }

       public async Task<bool> UpdateAsync(T model)
        {
           EntityEntry<T> entityEntry= Table.Update(model);
            return entityEntry.State==EntityState.Modified;
        }

        public async Task<int> SaveAsync() => await _context.SaveChangesAsync();
    }
}
