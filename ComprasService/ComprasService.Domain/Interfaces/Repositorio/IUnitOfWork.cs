﻿using System;

namespace ComprasService.Domain.Interfaces.Repository
{
    public interface IUnitOfWork: IDisposable
    {
        int SaveChanges();
        void BeginTransaction();
        void RollbackTransaction();
        void CommitTransaction();
        IGenericRepository<T> Crud<T>() where T : class;
    }
}
