using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace TechnicalSupportSystem.Repository
{
    public interface IRepository
    {
        void Save();

        void Dispose();

        IQueryable<T> Query<T>(Expression<Func<T, bool>> filter = null) where T : class;
        T SingleOrDefault<T>(Expression<Func<T, bool>> predicate) where T : class;
        void Insert<T>(T entity) where T : class;
        void Update<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
    }
}