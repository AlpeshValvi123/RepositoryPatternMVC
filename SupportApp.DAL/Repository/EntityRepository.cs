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
    /// <typeparam name="TEntity">Entity type</typeparam>
    public partial class EntityRepository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
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
        /// <typeparam name="TEntity">Entity type</typeparam>
        public void BulkDeleteEntities(IList<TEntity> entities)
        {
            foreach (var entity in entities)
                _context.Set<TEntity>().Remove(entity);
        }

        /// <summary>
        /// Performs insert records in a table
        /// </summary>
        /// <param name="entities">Entities for insert operation</param>
        /// <typeparam name="TEntity">Entity type</typeparam>
        public void BulkInsertEntities(IList<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                _context.Set<TEntity>().Add(entity);
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
        public virtual async Task<IList<TEntity>> Get(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            foreach (Expression<Func<TEntity, object>> include in includes)
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
        public virtual IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

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
        public virtual async Task<TEntity> GetFirstOrDefault(Expression<Func<TEntity, bool>> filter = null, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            foreach (Expression<Func<TEntity, object>> include in includes)
                query = query.Include(include);

            return await query.FirstOrDefaultAsync(filter);
        }

        /// <summary>
        /// Get entity by identifier
        /// </summary>
        /// <param name="id">Identitier</param>
        /// <returns>Entity</returns>
        public async Task<TEntity> GetById(Guid id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }

        /// <summary>
        /// Get entities
        /// </summary>
        /// <returns>Entities</returns>
        public IEnumerable<TEntity> GetAll()
        {
            return _context.Set<TEntity>().ToList();
        }

        /// <summary>
        /// Insert entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Entity</returns>
        public void Insert(TEntity entity)
        {
            if (entity is AuditableEntity auditableEntity)
            {
                auditableEntity.Created = DateTime.Now;
                auditableEntity.CreatedBy = entity.Id;
            }
            _context.Set<TEntity>().Add(entity);
        }

        /// <summary>
        /// Insert entity entries
        /// </summary>
        /// <param name="entities">Entity entries</param>
        public virtual void Insert(IList<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            BulkInsertEntities(entities);
        }

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public void Delete(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _context.Set<TEntity>().Remove(entity);
        }

        /// <summary>
        /// Delete entity entries
        /// </summary>
        /// <param name="entities">Entity entries</param>
        public virtual void Delete(IList<TEntity> entities)
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
        public virtual void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            var entities = _context.Set<TEntity>()
               .Where(predicate).ToList();

            BulkDeleteEntities(entities);
        }


        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Entity</returns>
        public void Update(TEntity entity)
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
        public virtual IPagedList<TEntity> GetAllPaged(Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null,
              int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
        {
            //var query = await _context.Set<TEntity>().ToListAsync();
            //IQueryable<TEntity> query = _context.Set<TEntity>();

            IQueryable<TEntity> query = func != null ? func(_context.Set<TEntity>()) : _context.Set<TEntity>();

            return new PagedList<TEntity>(query, pageIndex, pageSize, getOnlyTotalCount);
        }

        /// <summary>
        /// Get total count of the records
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetCount()
        {
            return await _context.Set<TEntity>().CountAsync();
        }

        /// <summary>
        /// Get entity entries by identifiers
        /// </summary>
        /// <param name="ids">Entity entry identifiers</param>
        /// <returns>Entity entries</returns>
        public virtual IList<TEntity> GetByIds(IList<Guid> ids)
        {
            if (!ids?.Any() ?? true)
                return new List<TEntity>();

            var query = _context.Set<TEntity>();

            //get entries
            var entries = query.Where(entry => ids.Contains(entry.Id)).ToList();

            //sort by passed identifiers
            var sortedEntries = new List<TEntity>();
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