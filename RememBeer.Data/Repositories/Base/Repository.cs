﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using RememBeer.Data.DbContexts.Contracts;

namespace RememBeer.Data.Repositories.Base
{
    public class Repository<T> : IRepository<T> where T : class
    {
        public Repository(IRememBeerMeDbContext context)
        {
            this.Context = context;
            this.DbSet = this.Context.Set<T>();
        }

        public IQueryable<T> All
        {
            get { return this.DbSet; }
        }

        public T GetById(object id)
        {
            return this.DbSet.Find(id);
        }

        public IEnumerable<T> GetAll()
        {
            return this.GetAll(null);
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> filterExpression)
        {
            return this.GetAll<object>(filterExpression, null);
        }

        public IEnumerable<T> GetAll<T1>(Expression<Func<T, bool>> filterExpression,
                                         Expression<Func<T, T1>> sortExpression)
        {
            return this.GetAll<T1, T>(filterExpression, sortExpression, null);
        }

        public IEnumerable<T2> GetAll<T1, T2>(Expression<Func<T, bool>> filterExpression,
                                              Expression<Func<T, T1>> sortExpression,
                                              Expression<Func<T, T2>> selectExpression)
        {
            IQueryable<T> result = this.DbSet;

            if (filterExpression != null)
            {
                result = result.Where(filterExpression);
            }

            if (sortExpression != null)
            {
                result = result.OrderBy(sortExpression);
            }

            if (selectExpression != null)
            {
                return result.Select(selectExpression).ToList();
            }
            else
            {
                return result.OfType<T2>().ToList();
            }
        }

        public IRememBeerMeDbContext Context { get; set; }

        protected IDbSet<T> DbSet { get; set; }

        public void Add(T entity)
        {
            var dbEntity = this.CopyStateFrom(entity);
            var entry = this.AttachIfDetached(dbEntity);
            entry.State = EntityState.Added;
        }

        public void Update(T entity)
        {
            this.DbSet.AddOrUpdate(entity);
            //var entry = this.AttachIfDetached(entity);
            //entry.State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            var entry = this.AttachIfDetached(entity);
            entry.State = EntityState.Deleted;
        }

        public void SaveChanges()
        {
            this.Context.SaveChanges();
        }

        private DbEntityEntry AttachIfDetached(T entity)
        {
            var entry = this.Context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                this.DbSet.Attach(entity);
            }

            return entry;
        }

        private T CopyStateFrom(T entity)
        {
            var dbEntity = this.DbSet.Create();
            var type = typeof(T);
            var properties = type.GetProperties();

            foreach (var srcProp in  properties)
            {
                if (!srcProp.CanRead)
                {
                    continue;
                }

                var targetProperty = type.GetProperty(srcProp.Name);
                if (targetProperty == null)
                {
                    continue;
                }

                if (!targetProperty.CanWrite)
                {
                    continue;
                }
                if (targetProperty.GetSetMethod(true) != null && targetProperty.GetSetMethod(true).IsPrivate)
                {
                    continue;
                }

                if ((targetProperty.GetSetMethod().Attributes & MethodAttributes.Static) != 0)
                {
                    continue;
                }

                if (!targetProperty.PropertyType.IsAssignableFrom(srcProp.PropertyType))
                {
                    continue;
                }

                targetProperty.SetValue(dbEntity, srcProp.GetValue(entity, null), null);
            }

            return dbEntity;
        }
    }
}
