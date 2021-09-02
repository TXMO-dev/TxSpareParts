using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TxSpareParts.Core.Interfaces;
using TxSpareParts.Infastructure.Data;

namespace TxSpareParts.Infastructure.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;
        public Repository(ApplicationDbContext db)
        {
            _db = db;
            dbSet = _db.Set<T>();
        }
        public async Task Add(T entity)
        {
            await dbSet.AddAsync(entity);
        }

        public async Task<T> Get(string id)
        {
           return await dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null)
        {
            IQueryable<T> query = dbSet;

            if(filter != null)
            {
                query = query.Where(filter);
                
            }

            if(orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }

            return await query.ToListAsync();

        }

        public async Task<T> GetFirstOrDefault(Expression<Func<T, bool>> filter = null)
        {
            IQueryable<T> query = dbSet;

            if(filter != null)
            {
                return await query.FirstOrDefaultAsync(filter);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async void Remove(string id)
        {
            var elementObj = await dbSet.FindAsync(id);
            Remove(elementObj);
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
        }
    }
}
