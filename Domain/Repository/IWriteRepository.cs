using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repository
{
    public interface IWriteRepository<T>:IRepository<T> where T : class, IEntityBase
    {
        Task<bool> AddAsync(T model);
        Task<bool> AddRangeAsync(List<T> model);
        bool Delete(T model);
        bool Delete(Guid id);
        bool DeleteRange(IEnumerable<T> models);
        Task<bool> UpdateAsync(T model);
        Task<int> SaveAsync();
    }
}
