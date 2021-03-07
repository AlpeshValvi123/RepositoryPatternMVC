using SupportApp.Core;
using SupportApp.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SupportApp.DAL.Repository
{
    /// <summary>
    /// Represents an entity repository
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public interface IRepository<T> where T : BaseEntity
    {
        /// <summary>
        /// Get all entities
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="orderBy"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        Task<IList<T>> Get(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            params Expression<Func<T, object>>[] includes);

        /// <summary>
        /// Get query for entity
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        IQueryable<T> Query(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null);

        /// <summary>
        /// Get first or default entity by filter
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        Task<T> GetFirstOrDefault(
            Expression<Func<T, bool>> filter = null,
            params Expression<Func<T, object>>[] includes);


        /// <summary>
        /// Get entity by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Entity</returns>
        Task<T> GetById(Guid id);

        /// <summary>
        /// Get entities
        /// </summary>
        /// <returns>Entities</returns>
        IEnumerable<T> GetAll();

        /// <summary>
        /// Get paged list of all entity entries
        /// </summary>
        /// <param name="func">Function to select entries</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="getOnlyTotalCount">Whether to get only the total number of entries without actually loading data</param>
        /// <returns>Paged list of entity entries</returns>
        IPagedList<T> GetAllPaged(Func<IQueryable<T>, IQueryable<T>> func = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);

        /// <summary>
        /// Get entity entries by identifiers
        /// </summary>
        /// <param name="ids">Entity entry identifiers</param>
        /// <returns>Entity entries</returns>
        IList<T> GetByIds(IList<Guid> ids);

        /// <summary>
        /// Get total count of the records
        /// </summary>
        /// <returns></returns>
        Task<int> GetCount();

        /// <summary>
        /// Insert entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Entity</returns>
        void Insert(T entity);

        /// <summary>
        /// Insert entity entries
        /// </summary>
        /// <param name="entities">Entity entries</param>
        void Insert(IList<T> entities);

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entity">Entity</param>
        void Delete(T entity);

        /// <summary>
        /// Delete entity entries
        /// </summary>
        /// <param name="entities">Entity entries</param>
        void Delete(IList<T> entities);

        /// <summary>
        /// Delete entity entries by the passed predicate
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition</param>
        void Delete(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Entity</returns>
        void Update(T entity);

        /// <summary>
        /// Save entity
        /// </summary>
        /// <returns>Count(retun 0 if succsess)</returns>
        Task<int> Save();
    }
}