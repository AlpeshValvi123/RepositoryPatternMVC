using SupportApp.Core;
using SupportApp.Core.Domain.Common;
using SupportApp.DAL.Persistence;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SupportApp.DAL.Repository
{
    /// <summary>
    /// Represents the entity repository implementation
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public partial class EntityRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly SupportAppDbContext _context;
        public EntityRepository(SupportAppDbContext context)
        {
            _context = context;
        }

        #region Utility
        /// <summary>
        /// Performs delete records in a table
        /// </summary>
        /// <param name="entities">Entities for delete operation</param>
        /// <typeparam name="T">Entity type</typeparam>
        public void BulkDeleteEntities(IList<T> entities)
        {
            foreach (var entity in entities)
                _context.Set<T>().Remove(entity);
        }

        /// <summary>
        /// Performs insert records in a table
        /// </summary>
        /// <param name="entities">Entities for insert operation</param>
        /// <typeparam name="T">Entity type</typeparam>
        public void BulkInsertEntities(IList<T> entities)
        {
            foreach (var entity in entities)
            {
                _context.Set<T>().Add(entity);
                _context.SaveChanges();
            }
        }

        #endregion

        /// <summary>
        /// Get all entities
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="orderBy"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        public virtual async Task<IList<T>> Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>();

            foreach (Expression<Func<T, object>> include in includes)
                query = query.Include(include);

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = orderBy(query);

            return await query.ToListAsync();
        }

        /// <summary>
        /// Get query for entity
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public virtual IQueryable<T> Query(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = orderBy(query);

            return query;
        }

        /// <summary>
        /// Get first or default entity by filter
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="includes"></param>
        public virtual async Task<T> GetFirstOrDefault(Expression<Func<T, bool>> filter = null, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>();

            foreach (Expression<Func<T, object>> include in includes)
                query = query.Include(include);

            return await query.FirstOrDefaultAsync(filter);
        }

        /// <summary>
        /// Get entity by identifier
        /// </summary>
        /// <param name="id">Identitier</param>
        /// <returns>Entity</returns>
        public async Task<T> GetById(Guid id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        /// <summary>
        /// Get entities
        /// </summary>
        /// <returns>Entities</returns>
        public IEnumerable<T> GetAll()
        {
            return _context.Set<T>().ToList();
        }

        /// <summary>
        /// Insert entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Entity</returns>
        public void Insert(T entity)
        {
            if (entity is AuditableEntity auditableEntity)
            {
                auditableEntity.Created = DateTime.Now;
                auditableEntity.CreatedBy = entity.Id;
            }
            _context.Set<T>().Add(entity);
        }

        /// <summary>
        /// Insert entity entries
        /// </summary>
        /// <param name="entities">Entity entries</param>
        public virtual void Insert(IList<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            BulkInsertEntities(entities);
        }

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public void Delete(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _context.Set<T>().Remove(entity);
        }

        /// <summary>
        /// Delete entity entries
        /// </summary>
        /// <param name="entities">Entity entries</param>
        public virtual void Delete(IList<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            if (entities.OfType<AuditableEntity>().Any())
            {
                foreach (var entity in entities)
                {
                    if (entity is AuditableEntity auditableEntity)
                    {
                        auditableEntity.LastModified = DateTime.Now;
                        auditableEntity.LastModifiedBy = entity.Id;
                        _context.Entry(entity);
                        _context.SaveChanges();
                    }
                }
            }
            else
            {
                BulkDeleteEntities(entities);
            }
        }

        /// <summary>
        /// Delete entity entries by the passed predicate
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition</param>
        public virtual void Delete(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            var entities = _context.Set<T>()
               .Where(predicate).ToList();

            BulkDeleteEntities(entities);
        }


        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Entity</returns>
        public void Update(T entity)
        {
            _context.Entry(entity);
        }

        /// <summary>
        /// Save entity
        /// </summary>
        /// <returns>Count(retun 0 if succsess)</returns>
        public async Task<int> Save()
        {
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Get paged list of all entity entries
        /// </summary>
        /// <param name="func">Function to select entries</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="getOnlyTotalCount">Whether to get only the total number of entries without actually loading data</param>
        /// <returns>Paged list of entity entries</returns>
        public virtual IPagedList<T> GetAllPaged(Func<IQueryable<T>, IQueryable<T>> func = null,
              int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
        {
            //var query = await _context.Set<T>().ToListAsync();
            //IQueryable<T> query = _context.Set<T>();

            IQueryable<T> query = func != null ? func(_context.Set<T>()) : _context.Set<T>();

            return new PagedList<T>(query, pageIndex, pageSize, getOnlyTotalCount);
        }

        /// <summary>
        /// Get total count of the records
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetCount()
        {
            return await _context.Set<T>().CountAsync();
        }

        /// <summary>
        /// Get entity entries by identifiers
        /// </summary>
        /// <param name="ids">Entity entry identifiers</param>
        /// <returns>Entity entries</returns>
        public virtual IList<T> GetByIds(IList<Guid> ids)
        {
            if (!ids?.Any() ?? true)
                return new List<T>();

            var query = _context.Set<T>();

            //get entries
            var entries = query.Where(entry => ids.Contains(entry.Id)).ToList();

            //sort by passed identifiers
            var sortedEntries = new List<T>();
            foreach (var id in ids)
            {
                var sortedEntry = entries.FirstOrDefault(entry => entry.Id == id);
                if (sortedEntry != null)
                    sortedEntries.Add(sortedEntry);
            }

            return sortedEntries;
        }

    }
}