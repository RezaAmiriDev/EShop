using ClassLibrary.Models;
using ClassLibrary.Repository;
using DataLayer.ApiResult;
using DataLayer.Base;
using Microsoft.EntityFrameworkCore;


namespace ModelLayer.Reposetotry
{
    public class Repos<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
    {
        protected readonly MobiContext _mobiContext;
        public virtual IQueryable<TEntity> Table => Entities;
        public virtual IQueryable<TEntity> TableNoTracking => Entities.AsNoTracking();
        public DbSet<TEntity> Entities { get; }

        public Repos(MobiContext mobiContext)
        {
            _mobiContext = mobiContext;
            Entities = _mobiContext.Set<TEntity>();
        }

       // Rang
        public virtual TEntity GetById(params object[] ids)
        {
            var find = Entities.Find(ids);
            return find!;
        }
        public virtual async Task<TEntity> GetByIdAsync(params object[] ids )
        {
            var entry = await Entities.FindAsync(ids);
            return entry!;
        }
        public virtual async Task<List<TEntity>> GetAllAsync(CancellationToken token = default)
        {
            return await TableNoTracking.ToListAsync(token);
        }

       // Add & AddRang
        public virtual async Task<ServiceResult> AddAsync(TEntity entity, CancellationToken token = default)
        {
            try
            {
                Guid newId = Guid.NewGuid();
                var prop = entity.GetType().GetProperty("Id");
                if (prop != null)
                {
                    var typeId = prop.PropertyType.Name;
                    if(typeId == "Guid")
                    {
                        prop.SetValue(entity, newId);
                    }
                }
                await Entities.AddAsync(entity , token);
                await _mobiContext.SaveChangesAsync(token);
                return new ServiceResult(ResponseStatus.Success, null);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    return new ServiceResult(ResponseStatus.BadRequest, null);
                }
                else
                {
                    return new ServiceResult(ResponseStatus.ServerError, null);
                }
            }
        }
        public virtual async Task<ServiceResult> AddRangeAsync(IEnumerable<TEntity> entities , CancellationToken token = default)
        {
            try
            {
                await Entities.AddRangeAsync(entities, token);
                await _mobiContext.SaveChangesAsync(token);
                return new ServiceResult(ResponseStatus.Success, null);
            }
            catch (Exception)
            {

                return new ServiceResult(ResponseStatus.ServerError, null);
            }
        }
 
       // Update & UpdateRang
        public virtual async Task<ServiceResult> UpdateAsync(TEntity entity , CancellationToken token = default)
        {
            try
            {
                Entities.Update(entity);
                await _mobiContext.SaveChangesAsync(token);
                return new ServiceResult(ResponseStatus.Success, null);
            }
            catch (Exception)
            {

                return new ServiceResult(ResponseStatus.ServerError, null);
            }
        }
        public virtual async Task<ServiceResult> UpdateRangeAsync(IEnumerable<TEntity> entities , CancellationToken token = default)
        {
            try
            {
                Entities.UpdateRange(entities);
                await _mobiContext.SaveChangesAsync(token);
                return new ServiceResult(ResponseStatus.Success, null);
            }
            catch (Exception)
            {

                return new ServiceResult(ResponseStatus.ServerError, null);
            }
        }

       // Delete & DeleteRang
        public virtual async Task<ServiceResult> DeleteAsync(TEntity entity , CancellationToken token = default)
        {
            try
            {
                Entities.Remove(entity);
                await _mobiContext.SaveChangesAsync(token);
                return new ServiceResult(ResponseStatus.Success, null);
            }
            catch (Exception)
            {

                return new ServiceResult(ResponseStatus.ServerError, null);
            }
        }
        public virtual async Task<ServiceResult> DeleteRangeAsync(IEnumerable<TEntity> entities , CancellationToken token = default)
        {
            try
            {
                Entities.RemoveRange(entities);
                await _mobiContext.SaveChangesAsync(token);
                return new ServiceResult(ResponseStatus.Success, null);
            }
            catch (Exception)
            {

                return new ServiceResult(ResponseStatus.ServerError, null);
            }
        }

        //موجودیت (entity) را به DbSet اضافه می‌کند (AddAsync).
        // با SaveChangesAsync تغییرات را در پایگاه داده ذخیره می‌کند.
        public virtual async Task<TEntity> InsertAndReturnAsync(TEntity entity , CancellationToken token = default)
        {
            try
            {
                await Entities.AddAsync(entity);
                await _mobiContext.SaveChangesAsync(token);
                return entity;
            }
            catch (DbUpdateException)
            {
                // لاگ کن
                // _logger?.LogError(dbEx, "Insert failed");
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}