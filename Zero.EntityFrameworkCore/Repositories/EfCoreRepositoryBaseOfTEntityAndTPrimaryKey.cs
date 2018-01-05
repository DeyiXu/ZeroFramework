using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Zero.Collections.Extensions;
using Zero.Domain.Entities;
using Zero.Domain.Entities.Auditing;
using Zero.Domain.Repositories;
using Zero.Domain.Uow;
using Zero.EntityFrameworkCore.Uow;
using Zero.Operator;
using Zero.Operator.Extensions;

namespace Zero.EntityFrameworkCore.Repositories
{
    public class EfCoreRepositoryBase<TDbContext, TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey>
        where TEntity : class,
        IEntity<TPrimaryKey> where TDbContext : DbContext
    {
        public TDbContext Context;

        private Dictionary<string, TDbContext> DicContext = new Dictionary<string, TDbContext>();

        private IUnitOfWork UnitOfWork;
        //public virtual DbSet<TEntity> Table => Context.Set<TEntity>();
        public virtual DbSet<TEntity> Table => Context.Set<TEntity>();
        public EfCoreRepositoryBase(TDbContext context)
        {
            Context = context;
            UnitOfWork = new UnitOfWork<DbContext>(context);
        }
        public virtual int Count()
        {
            return GetAll().Count();
        }

        public virtual int Count(Expression<Func<TEntity, bool>> predicate)
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

        public virtual int Delete(TEntity entity)
        {
            ApplyDeletionParameter(entity);
            Table.Remove(entity);

            return UnitOfWork.SaveChanges();
        }

        public virtual int Delete(TPrimaryKey id)
        {
            var entity = Get(id);
            ApplyDeletionParameter(entity);
            return Delete(entity);
            //return UnitOfWork.SaveChanges();
        }

        public virtual int Delete(Expression<Func<TEntity, bool>> predicate)
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

            return UnitOfWork.SaveChanges();
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

        public virtual TEntity FirstOrDefault(TPrimaryKey id)
        {
            return GetAll().FirstOrDefault(CreateEqualityExpressionForId(id));
        }

        public virtual TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().FirstOrDefault(predicate);
        }

        public virtual TEntity Get(TPrimaryKey id)
        {
            var entity = FirstOrDefault(id);
            return entity;
        }

        public virtual IQueryable<TEntity> GetAll()
        {
            return Table.AsQueryable();
        }


        public virtual IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] propertySelectors)
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

        public virtual List<TEntity> GetAllList()
        {
            return GetAll().ToList();
        }
        public virtual async Task<List<TEntity>> GetAllListAsync()
        {
            return await GetAll().ToListAsync();
        }

        public virtual List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Where(predicate).ToList();
        }

        public virtual TEntity Insert(TEntity entity)
        {
            ApplyCreationParameter(entity);
            Table.Add(entity);

            UnitOfWork.SaveChanges();

            return entity;
        }
        public virtual int Insert(List<TEntity> entitys)
        {
            foreach (var entity in entitys)
            {
                Table.Add(entity);
            }

            return UnitOfWork.SaveChanges();
        }
        public virtual TPrimaryKey InsertAndGetId(TEntity entity)
        {
            ApplyCreationParameter(entity);
            Context.Attach(entity);
            Context.Entry(entity).State = EntityState.Added;
            //Table.Add(entity);
            UnitOfWork.SaveChanges();
            return entity.Id;
        }

        public virtual TEntity InsertOrUpdate(TEntity entity)
        {
            return entity.IsTransient()
                ? Insert(entity)
                : Update(entity);
        }

        public virtual TPrimaryKey InsertOrUpdateAndGetId(TEntity entity)
        {
            return InsertOrUpdate(entity).Id;
        }

        public virtual TEntity Load(TPrimaryKey id)
        {
            return Get(id);
        }

        public virtual IQueryable<TEntity> LoadEntities<S>(Expression<Func<TEntity, bool>> whereLambda, bool isAsc, Expression<Func<TEntity, S>> orderByLambda)
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

        public virtual IQueryable<TEntity> LoadEntities(Expression<Func<TEntity, bool>> whereLambda)
        {
            return GetAll().Where(whereLambda).AsQueryable();
        }

        public virtual IQueryable<TEntity> LoadPageEntities<S>(int pageIndex, int pageSize, out int total, Expression<Func<TEntity, bool>> whereLambda, bool isAsc, Expression<Func<TEntity, S>> orderByLambda)
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

        public virtual long LongCount()
        {
            return GetAll().LongCount();
        }

        public virtual long LongCount(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Where(predicate).LongCount();
        }

        public virtual T Query<T>(Func<IQueryable<TEntity>, T> queryMethod)
        {
            return queryMethod(GetAll());
        }

        public virtual TEntity Single(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Single(predicate);
        }

        public virtual TEntity Update(TEntity entity)
        {
            ApplyModificationParameter(entity);
            Table.Update(entity);

            UnitOfWork.SaveChanges();
            return entity;
        }

        public virtual TEntity Update(TPrimaryKey id, Action<TEntity> updateAction)
        {
            var entity = Get(id);
            updateAction(entity);
            ApplyModificationParameter(entity);

            UnitOfWork.SaveChanges();
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

        public virtual async Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetAll().Where(predicate).ToListAsync();
        }

        public virtual async Task<TEntity> GetAsync(TPrimaryKey id)
        {
            return await Task.FromResult(Get(id));
        }

        public virtual async Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetAll().SingleAsync(predicate);
        }

        public virtual async Task<TEntity> FirstOrDefaultAsync(TPrimaryKey id)
        {
            return await GetAll().FirstOrDefaultAsync(CreateEqualityExpressionForId(id));
        }

        public virtual async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetAll().FirstOrDefaultAsync(predicate);
        }

        public virtual async Task<TEntity> InsertAsync(TEntity entity)
        {
            return await Task.FromResult(Insert(entity));
        }

        public virtual async Task<int> InsertAsync(List<TEntity> entitys)
        {
            return await Task.FromResult(Insert(entitys));
        }

        public virtual async Task<TPrimaryKey> InsertAndGetIdAsync(TEntity entity)
        {
            return await Task.FromResult(InsertAndGetId(entity));
        }

        public virtual async Task<TEntity> InsertOrUpdateAsync(TEntity entity)
        {
            return await Task.FromResult(InsertOrUpdate(entity));
        }

        public virtual async Task<TPrimaryKey> InsertOrUpdateAndGetIdAsync(TEntity entity)
        {
            return await Task.FromResult(InsertOrUpdateAndGetId(entity));
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity entity)
        {
            return await Task.FromResult(Update(entity));
        }

        public virtual async Task<TEntity> UpdateAsync(TPrimaryKey id, Action<TEntity> updateAction)
        {
            return await Task.FromResult(Update(id, updateAction));
        }

        public virtual async Task<int> DeleteAsync(TEntity entity)
        {
            return await Task.FromResult(Delete(entity));
        }

        public virtual async Task<int> DeleteAsync(TPrimaryKey id)
        {
            return await Task.FromResult(Delete(id));
        }

        public virtual async Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Task.FromResult(Delete(predicate));
        }

        public virtual async Task<int> CountAsync()
        {
            return await Task.FromResult(Count());
        }

        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Task.FromResult(Count(predicate));
        }

        public virtual async Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Task.FromResult(LongCount(predicate));
        }

        public virtual async Task<IQueryable<TEntity>> GetAllAsync()
        {
            return await Task.FromResult(GetAll());
        }
    }
}
