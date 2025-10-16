using Microsoft.EntityFrameworkCore;
using DataLayer.Base;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.ApiResult;

namespace ClassLibrary.Repository
{
    public interface IRepository<TEntity> where TEntity : class , IEntity
    {
        DbSet<TEntity> Entities { get; }
        IQueryable<TEntity> Table {  get; }
        IQueryable<TEntity> TableNoTracking { get; }
        Task<TEntity> InsertReturnInformation(TEntity entity);

        // add
        Task<ServiceResult> AddAsync(TEntity entity);
        Task<ServiceResult> AddRangeAsync(IEnumerable<TEntity?> entities);

        // delete
        Task<ServiceResult> DeleteAsync(TEntity entity);
        Task<ServiceResult> DeleteRangeAsync(IEnumerable<TEntity> entities);


        TEntity GetById(params object[] ids);
        Task<TEntity> GetByIdAsync(params object[] ids);

        // update
        Task<ServiceResult> UpdateAsync(TEntity entity);
        Task<ServiceResult> UpdateRangeAsync(IEnumerable<TEntity?> entities);


       // Attach/Detach (optional, keep if you need explicit state control)
       // void Datach(TEntity entity);
       // void Attach(TEntity entity);
    }
}
