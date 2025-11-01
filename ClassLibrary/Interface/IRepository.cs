using Microsoft.EntityFrameworkCore;
using DataLayer.Base;
using DataLayer.ApiResult;

namespace ClassLibrary.Repository
{
    public interface IRepository<TEntity> where TEntity : class , IEntity
    {
        DbSet<TEntity> Entities { get; }
        IQueryable<TEntity> Table {  get; }
        IQueryable<TEntity> TableNoTracking { get; }
        Task<TEntity> InsertAndReturnAsync(TEntity entity, CancellationToken token = default);

        // read or get
        TEntity GetById(params object[] ids);
        Task<TEntity> GetByIdAsync(params object[] ids);
        Task<List<TEntity>> GetAllAsync(CancellationToken token = default);

        // add
        Task<ServiceResult> AddAsync(TEntity entity , CancellationToken token = default);
        Task<ServiceResult> AddRangeAsync(IEnumerable<TEntity?> entities , CancellationToken token = default);

        // update
        Task<ServiceResult> UpdateAsync(TEntity entity , CancellationToken token = default);
        Task<ServiceResult> UpdateRangeAsync(IEnumerable<TEntity?> entities , CancellationToken token = default);

        // delete
        Task<ServiceResult> DeleteAsync(TEntity entity, CancellationToken token = default);
        Task<ServiceResult> DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken token = default);

    }
}
