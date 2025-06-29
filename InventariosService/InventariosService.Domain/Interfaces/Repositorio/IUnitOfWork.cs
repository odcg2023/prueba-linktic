using System;
using InventariosService.Domain.Interfaces.Repositorio;

namespace InventariosService.Domain.Interfaces.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        int SaveChanges();
        IGenericRepository<T> Crud<T>() where T : class;
        ITransaction BeginTransaction();
    }
}
