using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repository
{
    public interface IReadRepository<T>:IRepository<T> where T :class,IEntityBase
    {
        public IQueryable<T> GetAll();
        public IQueryable<T> GetWhere(Expression<Func<T, bool>> method);
        public Task<T> GetSingleAsync(Expression<Func<T, bool>> method);
        public Task<T> GetByIdAsync(Guid id);
    }
}
