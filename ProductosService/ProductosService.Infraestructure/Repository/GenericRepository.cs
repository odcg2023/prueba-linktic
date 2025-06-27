using Microsoft.EntityFrameworkCore;
using ProductosService.Domain.Interfaces.Repository;
using ProductosService.Infraestructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ProductosService.Infraestructure.Repository
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        public readonly ContextProductos _context;
        protected readonly DbSet<TEntity> _entity;

        public GenericRepository(ContextProductos context)
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

        public IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            IQueryable<TEntity> query = _entity.Where(predicate);
            return query;
        }

        public IEnumerable<TEntity> Get(Func<TEntity, bool> predicate)
        {
            return _entity.Where(predicate);
        }

        public IQueryable<TEntity> GetAll()
        {
            IQueryable<TEntity> query = _entity;
            return query;
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
