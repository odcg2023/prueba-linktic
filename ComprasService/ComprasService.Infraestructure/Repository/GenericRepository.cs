using Microsoft.EntityFrameworkCore;
using ComprasService.Domain.Interfaces.Repository;
using ComprasService.Infraestructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ComprasService.Infraestructure.Repository
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        public readonly ContextCompras _context;
        protected readonly DbSet<TEntity> _entity;

        public GenericRepository(ContextCompras context)
        {
            this._context = context;
            _entity = this._context.Set<TEntity>();
        }

        public TEntity Add(TEntity entity)
        {
            _entity.Add(entity);
            return entity;
        }

        public void Delete(TEntity entity)
        {
            _entity.Remove(entity);
        }

        public void Edit(TEntity entity)
        {
            _entity.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public IQueryable<TEntity> GetAll()
        {
            return _entity;
        }

        public IQueryable<TEntity> GetAllWithIncludes(params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _entity;
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return query;
        }

        public IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return _entity.Where(predicate);
        }

        public IQueryable<TEntity> FindWithIncludes(
            Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _entity;
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return query.Where(predicate);
        }

        public IEnumerable<TEntity> Get(Func<TEntity, bool> predicate)
        {
            return _entity.Where(predicate);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }

}
