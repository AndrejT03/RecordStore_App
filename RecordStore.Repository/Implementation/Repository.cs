using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using RecordStore.Domain.DomainModels;
using RecordStore.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RecordStore.Repository.Implementation
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> entites;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            this.entites = _context.Set<T>();
        }
        public IQueryable<T> GetAll()
        {
            return entites.AsQueryable();
        }
        public T Insert(T entity)
        {
            _context.Add(entity);
            _context.SaveChanges();
            return entity;
        }

        public ICollection<T> InsertMany(ICollection<T> entity)
        {
            _context.AddRange(entity);
            _context.SaveChanges();
            return entity;
        }

        public T Update(T entity)
        {
            _context.Update(entity);
            _context.SaveChanges();
            return entity;
        }

        public T Delete(T entity)
        {
            _context.Remove(entity);
            _context.SaveChanges();
            return entity;
        }

        public E? Get<E>(Expression<Func<T, E>> selector,
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
        {
            IQueryable<T> query = entites;
            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (include != null)
            {
                query = query.AsSplitQuery();
            }

            if (orderBy != null)
            {
                return orderBy(query).Select(selector).FirstOrDefault();
            }

            return query.Select(selector).FirstOrDefault();
        }

        public IEnumerable<E> GetAll<E>(Expression<Func<T, E>> selector,
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
        {
            IQueryable<T> query = entites;

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (include != null)
            {
                query = query.AsSplitQuery();
            }

            if (orderBy != null)
            {
                return orderBy(query).Select(selector).AsEnumerable();
            }

            return query.Select(selector).AsEnumerable();
        }
        public IQueryable<T> GetAllAsQueryable(Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
        {
            IQueryable<T> query = entites;

            if (include != null)
            {
                query = include(query);
            }

            return query;
        }

    }
}
