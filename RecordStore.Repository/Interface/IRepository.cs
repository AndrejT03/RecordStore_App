using Microsoft.EntityFrameworkCore.Query;
using RecordStore.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace RecordStore.Repository.Interface
{
    public interface IRepository<T> where T : BaseEntity
    {
        IQueryable<T> GetAll();
        T Insert(T entity);
        ICollection<T> InsertMany(ICollection<T> entity);
        T Update(T entity);
        T Delete(T entity);
        E? Get<E>(Expression<Func<T, E>> selector,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null);

        IEnumerable<E> GetAll<E>(Expression<Func<T, E>> selector,
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null);

        IQueryable<T> GetAllAsQueryable(Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null);
    }
}
