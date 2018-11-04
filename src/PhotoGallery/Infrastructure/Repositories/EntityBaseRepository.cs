using PhotoGallery.Entities;
using PhotoGallery.Infrastructure.Repositories.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace PhotoGallery.Infrastructure.Repositories
{
    public class EntityBaseRepository<T> : IEntityBaseRepository<T> where T : class, IEntityBase, new()
    {
        private PhotoGalleryContext context;

        public EntityBaseRepository(PhotoGalleryContext cont)
        {
            this.context = cont;
        }

        public virtual IEnumerable<T> GetAll()
        {
            return this.context.Set<T>().AsEnumerable();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await this.context.Set<T>().ToListAsync();
        }

        public virtual IEnumerable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = this.context.Set<T>();
            foreach (var item in includeProperties)
            {
                query = query.Include(item);
            }

            return query.AsEnumerable();
        }

        public virtual async Task<IEnumerable<T>> AllIncludingAsync(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = this.context.Set<T>();
            foreach (var item in includeProperties)
            {
                query = query.Include(item);
            }

            return await query.ToListAsync();
        }

        public T GetSingle(Expression<Func<T, bool>> predicate)
        {
            return this.context.Set<T>().FirstOrDefault(predicate);
        }

        public T GetSingle(int id)
        {
            return this.context.Set<T>().FirstOrDefault(x => x.Id == id);
        }

        public T GetSingle(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable query = this.context.Set<T>();
            foreach (var item in includeProperties)
            {
                query =
            }
        }

        public Task<T> GetSingleAsync(int id)
        {
            throw new NotImplementedException();
        }

        public void Add(T entity)
        {
            throw new NotImplementedException();
        }

        

        

        public void Commit()
        {
            throw new NotImplementedException();
        }

        public void Delete(T entity)
        {
            throw new NotImplementedException();
        }

        public void Edit(T entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> FindByAsync(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        

       
    }
}
