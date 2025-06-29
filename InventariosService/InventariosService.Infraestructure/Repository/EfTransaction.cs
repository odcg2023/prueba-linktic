using InventariosService.Domain.Interfaces.Repositorio;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventariosService.Infraestructure.Repository
{
    public class EfTransaction : ITransaction
    {
        private readonly IDbContextTransaction _transaction;

        public EfTransaction(IDbContextTransaction transaction)
        {
            _transaction = transaction;
        }

        public void Commit() => _transaction.Commit();
        public void Rollback() => _transaction.Rollback();
        public void Dispose() => _transaction.Dispose();
    }
}
