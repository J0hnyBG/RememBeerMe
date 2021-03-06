﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using RememBeer.Data.Repositories.Enums;

namespace RememBeer.Data.Repositories.Base
{
    public interface IRepository<T> where T : class
    {
        T GetById(object id);

        IQueryable<T> All { get; }

        IEnumerable<T> GetAll();

        IEnumerable<T> GetAll(Expression<Func<T, bool>> filterExpression);

        IEnumerable<T> GetAll<T1>(Expression<Func<T, bool>> filterExpression,
                                  Expression<Func<T, T1>> sortExpression,
                                  SortOrder? sortOrder);

        IEnumerable<T2> GetAll<T1, T2>(Expression<Func<T, bool>> filterExpression,
                                       Expression<Func<T, T1>> sortExpression,
                                       SortOrder? sortOrder,
                                       Expression<Func<T, T2>> selectExpression);

        void Add(T entity);

        void Update(T entity);

        void Delete(T entity);

        IDataModifiedResult SaveChanges();
    }
}
