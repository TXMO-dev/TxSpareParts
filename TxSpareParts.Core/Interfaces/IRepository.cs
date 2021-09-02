using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TxSpareParts.Core.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task Add(T entity);  
        Task<T> Get(string id);   
        Task<IEnumerable<T>> GetAll(
            Expression<Func<T,bool>> filter = null,   
            Func<IQueryable<T>,IOrderedQueryable<T>> orderBy = null
            );
        Task<T> GetFirstOrDefault(
            Expression<Func<T, bool>> filter = null
            );  
        void Remove(string id);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
    }
}
