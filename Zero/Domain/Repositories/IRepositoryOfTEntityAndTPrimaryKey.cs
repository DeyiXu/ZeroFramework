using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Zero.Domain.Entities;

namespace Zero.Domain.Repositories
{
    public interface IRepository<TEntity, TPrimaryKey> : IRepository where TEntity : class, IEntity<TPrimaryKey>
    {
        //IRepository<TEntity, TPrimaryKey> BeginTrans();
        //int Commit();

        #region Select/Get/Query
        IQueryable<TEntity> GetAll();
        Task<IQueryable<TEntity>> GetAllAsync();
        IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] propertySelectors);

        List<TEntity> GetAllList();

        Task<List<TEntity>> GetAllListAsync();

        List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate);

        Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate);

        T Query<T>(Func<IQueryable<TEntity>, T> queryMethod);

        TEntity Get(TPrimaryKey id);

        Task<TEntity> GetAsync(TPrimaryKey id);

        TEntity Single(Expression<Func<TEntity, bool>> predicate);

        Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate);

        TEntity FirstOrDefault(TPrimaryKey id);

        Task<TEntity> FirstOrDefaultAsync(TPrimaryKey id);

        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);

        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

        TEntity Load(TPrimaryKey id);
        #endregion

        #region Paged
        IQueryable<TEntity> LoadEntities<S>(Expression<Func<TEntity, bool>> whereLambda, bool isAsc, Expression<Func<TEntity, S>> orderByLambda);
        IQueryable<TEntity> LoadEntities(Expression<Func<TEntity, bool>> whereLambda);
        IQueryable<TEntity> LoadPageEntities<S>(int pageIndex, int pageSize, out int total, Expression<Func<TEntity, bool>> whereLambda, bool isAsc, Expression<Func<TEntity, S>> orderByLambda);
        #endregion

        #region Insert
        TEntity Insert(TEntity entity);

        Task<TEntity> InsertAsync(TEntity entity);

        int Insert(List<TEntity> entitys);

        Task<int> InsertAsync(List<TEntity> entitys);

        TPrimaryKey InsertAndGetId(TEntity entity);

        Task<TPrimaryKey> InsertAndGetIdAsync(TEntity entity);

        TEntity InsertOrUpdate(TEntity entity);

        Task<TEntity> InsertOrUpdateAsync(TEntity entity);

        TPrimaryKey InsertOrUpdateAndGetId(TEntity entity);

        Task<TPrimaryKey> InsertOrUpdateAndGetIdAsync(TEntity entity);

        #endregion

        #region Update
        TEntity Update(TEntity entity);

        Task<TEntity> UpdateAsync(TEntity entity);

        TEntity Update(TPrimaryKey id, Action<TEntity> updateAction);

        Task<TEntity> UpdateAsync(TPrimaryKey id, Action<TEntity> updateAction);

        #endregion

        #region Delete

        int Delete(TEntity entity);

        Task<int> DeleteAsync(TEntity entity);

        int Delete(TPrimaryKey id);

        Task<int> DeleteAsync(TPrimaryKey id);

        int Delete(Expression<Func<TEntity, bool>> predicate);

        Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate);

        #endregion

        #region Aggregates

        int Count();

        Task<int> CountAsync();

        int Count(Expression<Func<TEntity, bool>> predicate);

        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);

        long LongCount();

        long LongCount(Expression<Func<TEntity, bool>> predicate);

        Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate);

        #endregion
    }
}
