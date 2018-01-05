﻿using Zero.Domain.Entities;

namespace Zero.Domain.Repositories
{
    public interface IRepository<TEntity> : IRepository<TEntity, int> where TEntity : class, IEntity<int>
    {

    }
}
