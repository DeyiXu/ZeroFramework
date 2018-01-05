using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Zero.Domain.Entities;
using Zero.Domain.Repositories;
using System.Linq;
using System.Linq.Expressions;
using Zero.Collections.Extensions;
using System.Reflection;
using Zero.Domain.Entities.Auditing;
using Zero.Operator;
using Microsoft.EntityFrameworkCore.Storage;
using Zero.Domain.Uow;
using Zero.EntityFrameworkCore.Uow;
using Zero.Operator.Extensions;
using System.Threading.Tasks;

namespace Zero.EntityFrameworkCore.Repositories2
{
    public class EfCoreRepositoryBase<TDbContext, TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey>
        where TEntity : class,
        IEntity<TPrimaryKey> where TDbContext : DbContext
    {
        public TDbContext Context;
        private IUnitOfWork UnitOfWork;
        public virtual DbSet<TEntity> Table => Context.Set<TEntity>();
        public EfCoreRepositoryBase(TDbContext context)
        {
            Context = context;
            UnitOfWork = new UnitOfWork<DbContext>(context);
        }

        public int Count()
        {
            return GetAll().Count();
        }

        public int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Count(predicate);
        }

        protected virtual void AttachIfNot(TEntity entity)
        {
            var entry = Context.ChangeTracker.Entries().FirstOrDefault(ent => ent.Entity == entity);
            if (entry != null)
            {
                return;
            }
            Table.Attach(entity);
        }

        public int Delete(TEntity entity)
        {
            ApplyDeletionParameter(entity);
            Table.Remove(entity);
            return UnitOfWork.Commit();
        }

        public int Delete(TPrimaryKey id)
        {
            var entity = Get(id);
            ApplyDeletionParameter(entity);
            Delete(entity);
            return UnitOfWork.Commit();
        }

        public int Delete(Expression<Func<TEntity, bool>> predicate)
        {
            foreach (var entity in GetAll().Where(predicate).ToList())
            {
                bool flag = ApplyDeletionParameter(entity);

                if (flag)
                {
                    Table.Update(entity);
                }
                else
                {
                    Table.Remove(entity);
                }

            }

            return UnitOfWork.Commit();
        }

        protected static Expression<Func<TEntity, bool>> CreateEqualityExpressionForId(TPrimaryKey id)
        {
            var lambdaParam = Expression.Parameter(typeof(TEntity));

            var lambdaBody = Expression.Equal(
                Expression.PropertyOrField(lambdaParam, "Id"),
                Expression.Constant(id, typeof(TPrimaryKey))
                );

            return Expression.Lambda<Func<TEntity, bool>>(lambdaBody, lambdaParam);
        }

        public TEntity FirstOrDefault(TPrimaryKey id)
        {
            return GetAll().FirstOrDefault(CreateEqualityExpressionForId(id));
        }

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().FirstOrDefault(predicate);
        }

        public TEntity Get(TPrimaryKey id)
        {
            var entity = FirstOrDefault(id);
            return entity;
        }

        public IQueryable<TEntity> GetAll()
        {
            return Table.AsQueryable();
        }

        public IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            var query = Table.AsQueryable();

            if (!propertySelectors.IsNullOrEmpty())
            {
                foreach (var propertySelector in propertySelectors)
                {
                    query = query.Include(propertySelector);
                }
            }

            query = ApplyFilters(query);

            return query;

        }

        public List<TEntity> GetAllList()
        {
            return GetAll().ToList();
        }
        public async Task<List<TEntity>> GetAllListAsync()
        {
            return await GetAll().ToListAsync();
        }

        public List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Where(predicate).ToList();
        }

        public TEntity Insert(TEntity entity)
        {
            ApplyCreationParameter(entity);
            Table.Add(entity);

            UnitOfWork.Commit();

            return entity;
        }
        public int Insert(List<TEntity> entitys)
        {
            foreach (var entity in entitys)
            {
                Table.Add(entity);
            }

            return UnitOfWork.Commit();
        }
        public TPrimaryKey InsertAndGetId(TEntity entity)
        {
            ApplyCreationParameter(entity);
            Table.Add(entity);
            UnitOfWork.Commit();
            return entity.Id;
        }

        public TEntity InsertOrUpdate(TEntity entity)
        {
            return entity.IsTransient()
                ? Insert(entity)
                : Update(entity);
        }

        public TPrimaryKey InsertOrUpdateAndGetId(TEntity entity)
        {
            return InsertOrUpdate(entity).Id;
        }

        public TEntity Load(TPrimaryKey id)
        {
            return Get(id);
        }

        public IQueryable<TEntity> LoadEntities<S>(Expression<Func<TEntity, bool>> whereLambda, bool isAsc, Expression<Func<TEntity, S>> orderByLambda)
        {
            if (isAsc)
            {
                var temp = GetAll().Where(whereLambda).OrderBy(orderByLambda).AsQueryable();
                return temp;
            }
            else
            {
                var temp = GetAll().Where(whereLambda).OrderByDescending(orderByLambda).AsQueryable();
                return temp;
            }
        }

        public IQueryable<TEntity> LoadEntities(Expression<Func<TEntity, bool>> whereLambda)
        {
            return GetAll().Where(whereLambda).AsQueryable();
        }

        public IQueryable<TEntity> LoadPageEntities<S>(int pageIndex, int pageSize, out int total, Expression<Func<TEntity, bool>> whereLambda, bool isAsc, Expression<Func<TEntity, S>> orderByLambda)
        {
            total = GetAll().Where(whereLambda).Count();
            if (isAsc)
            {
                var temp = GetAll().Where(whereLambda)
                     .OrderBy(orderByLambda)
                     .Skip(pageSize * (pageIndex - 1))
                     .Take(pageSize);
                return temp.AsQueryable();
            }
            else
            {
                var temp = GetAll().Where(whereLambda)
                     .OrderByDescending(orderByLambda)
                     .Skip(pageSize * (pageIndex - 1))
                     .Take(pageSize);
                return temp.AsQueryable();
            }
        }

        public long LongCount()
        {
            return GetAll().LongCount();
        }

        public long LongCount(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Where(predicate).LongCount();
        }

        public T Query<T>(Func<IQueryable<TEntity>, T> queryMethod)
        {
            return queryMethod(GetAll());
        }

        public TEntity Single(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Single(predicate);
        }

        public TEntity Update(TEntity entity)
        {
            ApplyModificationParameter(entity);
            Table.Update(entity);

            UnitOfWork.Commit();
            return entity;
        }

        public TEntity Update(TPrimaryKey id, Action<TEntity> updateAction)
        {
            var entity = Get(id);
            updateAction(entity);
            ApplyModificationParameter(entity);

            UnitOfWork.Commit();
            return entity;
        }

        #region Filters
        protected virtual IQueryable<TEntity> ApplyFilters(IQueryable<TEntity> query)
        {
            query = ApplySoftDeleteFilter(query);
            return query;
        }

        private bool IsTEntityFrom(Type type)
        {
            return type.GetTypeInfo().IsAssignableFrom(typeof(TEntity));
        }

        private IQueryable<TEntity> ApplySoftDeleteFilter(IQueryable<TEntity> query)
        {
            if (IsTEntityFrom(typeof(ISoftDelete)))
            {
                query = query.Where(e => !((ISoftDelete)e).IsDeleted);
            }
            return query;
        }

        private bool ApplyDeletionParameter(TEntity entity)
        {
            bool flag = false;
            if (IsTEntityFrom(typeof(IDeletionAudited)))
            {
                var provider = OperatorProvider.Provider.GetCurrent();
                ((IDeletionAudited)entity).DeleterUserId = provider.IsNullOrEmpty() ? 0 : provider.UserId;
                ((IDeletionAudited)entity).DeletionTime = DateTime.Now;
                ((IDeletionAudited)entity).IsDeleted = true;
                flag = true;
            }
            else if (IsTEntityFrom(typeof(IHasDeletionTime)))
            {
                ((IHasDeletionTime)entity).DeletionTime = DateTime.Now;
                ((IHasDeletionTime)entity).IsDeleted = true;
                flag = true;
            }
            else if (IsTEntityFrom(typeof(ISoftDelete)))
            {
                ((ISoftDelete)entity).IsDeleted = true;
                flag = true;
            }
            return flag;
        }

        private void ApplyModificationParameter(TEntity entity)
        {
            if (IsTEntityFrom(typeof(IModificationAudited)))
            {
                ((IModificationAudited)entity).LastModificationTime = DateTime.Now;
                var provider = OperatorProvider.Provider.GetCurrent();
                ((IModificationAudited)entity).LastModifierUserId = provider.IsNullOrEmpty() ? 0 : provider.UserId;
            }
            else if (IsTEntityFrom(typeof(IHasModificationTime)))
            {
                ((IHasModificationTime)entity).LastModificationTime = DateTime.Now;
            }
        }

        private void ApplyCreationParameter(TEntity entity)
        {
            if (IsTEntityFrom(typeof(ICreationAudited)))
            {
                ((ICreationAudited)entity).CreationTime = DateTime.Now;
                var provider = OperatorProvider.Provider.GetCurrent();
                ((ICreationAudited)entity).CreatorUserId = provider.IsNullOrEmpty() ? 0 : provider.UserId;
            }
            else if (IsTEntityFrom(typeof(IHasCreationTime)))
            {
                ((IHasCreationTime)entity).CreationTime = DateTime.Now;
            }
        }

        /// <summary>
        /// 应用创建时间和修改时间
        /// </summary>
        /// <param name="entity"></param>
        private void ApplyCreationOrModificationTime(TEntity entity)
        {
            if (entity.IsTransient())
            {
                ApplyCreationParameter(entity);
            }
            else
            {
                ApplyModificationParameter(entity);
            }
        }
        #endregion

        public async Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetAll().Where(predicate).ToListAsync();
        }

        public Task<TEntity> GetAsync(TPrimaryKey id)
        {
            return Task.FromResult(Get(id));
        }

        public async Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetAll().SingleAsync(predicate);
        }

        public async Task<TEntity> FirstOrDefaultAsync(TPrimaryKey id)
        {
            return await GetAll().FirstOrDefaultAsync(CreateEqualityExpressionForId(id));
        }

        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetAll().FirstOrDefaultAsync(predicate);
        }

        public Task<TEntity> InsertAsync(TEntity entity)
        {
            return Task.FromResult(Insert(entity));
        }

        public Task<int> InsertAsync(List<TEntity> entitys)
        {
            return Task.FromResult(Insert(entitys));
        }

        public Task<TPrimaryKey> InsertAndGetIdAsync(TEntity entity)
        {
            return Task.FromResult(InsertAndGetId(entity));
        }

        public Task<TEntity> InsertOrUpdateAsync(TEntity entity)
        {
            return Task.FromResult(InsertOrUpdate(entity));
        }

        public Task<TPrimaryKey> InsertOrUpdateAndGetIdAsync(TEntity entity)
        {
            return Task.FromResult(InsertOrUpdateAndGetId(entity));
        }

        public Task<TEntity> UpdateAsync(TEntity entity)
        {
            return Task.FromResult(Update(entity));
        }

        public Task<TEntity> UpdateAsync(TPrimaryKey id, Action<TEntity> updateAction)
        {
            return Task.FromResult(Update(id, updateAction));
        }

        public Task<int> DeleteAsync(TEntity entity)
        {
            return Task.FromResult(Delete(entity));
        }

        public Task<int> DeleteAsync(TPrimaryKey id)
        {
            return Task.FromResult(Delete(id));
        }

        public Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Task.FromResult(Delete(predicate));
        }

        public Task<int> CountAsync()
        {
            return Task.FromResult(Count());
        }

        public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Task.FromResult(Count(predicate));
        }

        public Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Task.FromResult(LongCount(predicate));
        }


    }
}
