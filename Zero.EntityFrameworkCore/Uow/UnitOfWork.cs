using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Zero.Domain.Uow;

namespace Zero.EntityFrameworkCore.Uow
{
    public class UnitOfWork<TDbContext> : IUnitOfWork, IDisposable where TDbContext : DbContext
    {
        private IDbContextTransaction DbTransaction { get; set; }
        private TDbContext Context;
        private Dictionary<int, TDbContext> DicContext = new Dictionary<int, TDbContext>();
        public UnitOfWork(TDbContext context)
        {
            Context = context;
        }
        public IUnitOfWork BeginTransaction()
        {
            DbTransaction = Context.Database.BeginTransaction();
            return this;
        }

        //public int Commit()
        //{
        //    try
        //    {
        //        var returnValue = 0;
        //        if (DbTransaction == null)
        //        {
        //            returnValue = this.SaveChanges();
        //        }
        //        else if (DbTransaction != null)
        //        {
        //            DbTransaction.Commit();
        //        }
        //        return returnValue;
        //    }
        //    catch (Exception ex)
        //    {
        //        this.Rollback();
        //        throw ex;
        //    }
        //    //finally
        //    //{
        //    //    this.Dispose();
        //    //}
        //}
        public void Commit()
        {
            try
            {
                if (DbTransaction != null)
                {
                    DbTransaction.Commit();
                }
            }
            catch (Exception ex)
            {
                this.Rollback();
                throw ex;
            }
            //finally
            //{
            //    this.Dispose();
            //}
        }

        public void Dispose()
        {
            if (DbTransaction != null)
            {
                this.DbTransaction.Dispose();
            }
        }

        public void Rollback()
        {
            if (DbTransaction != null)
            {
                this.DbTransaction.Rollback();
            }
        }

        public int SaveChanges()
        {
            try
            {
                return Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            try
            {
                return await Context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
