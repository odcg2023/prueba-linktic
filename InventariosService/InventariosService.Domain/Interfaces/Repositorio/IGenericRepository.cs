using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace InventariosService.Domain.Interfaces.Repository
{
    public interface IGenericRepository<T> where T: class
    {
        IQueryable<T> GetAll();
        IQueryable<T> GetAllWithIncludes(params Expression<Func<T, object>>[] includes);
        IQueryable<T> Find(Expression<Func<T, bool>> predicate);
        IQueryable<T> FindWithIncludes(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes);
        IEnumerable<T> Get(Func<T, bool> predicate);
        T Add(T entity);
        void Delete(T entity);
        void Edit(T entity);
        void Save();
    }
}
