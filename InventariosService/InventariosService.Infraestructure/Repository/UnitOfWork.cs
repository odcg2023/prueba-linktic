using Microsoft.EntityFrameworkCore;
using InventariosService.Domain.Interfaces.Repository;
using InventariosService.Infraestructure.Context;
using System;
using System.Collections.Generic;
using System.Text;
using InventariosService.Domain.Interfaces.Repositorio;

namespace InventariosService.Infraestructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ContextInventarios _context;

        public UnitOfWork(ContextInventarios context)
        {
            _context = context;
        }

        public IGenericRepository<T> Crud<T>() where T : class
        {
            return new GenericRepository<T>(_context);
        }

        public ITransaction BeginTransaction()
        {
            var transaction = _context.Database.BeginTransaction();
            return new EfTransaction(transaction);
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
