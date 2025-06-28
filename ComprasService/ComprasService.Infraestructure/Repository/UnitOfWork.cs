using Microsoft.EntityFrameworkCore;
using ComprasService.Domain.Interfaces.Repository;
using ComprasService.Infraestructure.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace ComprasService.Infraestructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ContextCompras _context;

        public UnitOfWork(ContextCompras context)
        {
            this._context = context;
        }

        public IGenericRepository<T> Crud<T>() where T : class
        {
            return new GenericRepository<T>(_context);
        }

        public void Dispose()
        {
            if (_context != null)
            {
                _context.Dispose();
            }
        }

        public void BeginTransaction()
        {
            _context.Database.BeginTransaction();
        }

        public void RollbackTransaction()
        {
            _context.Database.RollbackTransaction();
        }

        public void CommitTransaction()
        {
            _context.Database.CommitTransaction();
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }
    }
}
