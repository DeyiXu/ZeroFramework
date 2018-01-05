using System;
using System.Threading.Tasks;

namespace Zero.Domain.Uow
{
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// 提供事务接口
        /// </summary>
        IUnitOfWork BeginTransaction();
        int SaveChanges();
        Task<int> SaveChangesAsync();
        //int Commit();
        void Commit();
        void Rollback();
    }
}
