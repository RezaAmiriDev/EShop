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

        #region Add Method
        public virtual async Task<ServiceResult> AddAsync(TEntity entity)
        {
            try
            {
                Guid newId = Guid.NewGuid();
                var typeId = entity.GetType().GetProperty("Id").PropertyType.Name;
                if (typeId == "Guid")
                {
                    entity.GetType().GetProperty("Id").SetValue(entity, newId);
                }
                await Entities.AddAsync(entity);
                await _mobiContext.SaveChangesAsync();
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
        public async Task<ServiceResult> AddRangeAsync(IEnumerable<TEntity> entities)
        {
            try
            {
                await Entities.AddRangeAsync(entities);
                await _mobiContext.SaveChangesAsync();
                return new ServiceResult(ResponseStatus.Success, null);
            }
            catch (Exception)
            {

                return new ServiceResult(ResponseStatus.ServerError, null);
            }
        }
        #endregion

        #region Delete Method
        public virtual async Task<ServiceResult> DeleteAsync(TEntity entity)
        {
            try
            {
                Entities.Remove(entity);
                await _mobiContext.SaveChangesAsync();
                return new ServiceResult(ResponseStatus.Success, null);
            }
            catch (Exception)
            {

                return new ServiceResult(ResponseStatus.ServerError, null);
            }
        }
        public virtual async Task<ServiceResult> DeleteRangeAsync(IEnumerable<TEntity> entities)
        {
            try
            {
                Entities.RemoveRange(entities);
                await _mobiContext.SaveChangesAsync();
                return new ServiceResult(ResponseStatus.Success, null);
            }
            catch (Exception)
            {

                return new ServiceResult(ResponseStatus.ServerError, null);
            }
        }
        #endregion

        #region UpDate Method
        public virtual async Task<ServiceResult> UpdateAsync(TEntity entity)
        {
            try
            {
                Entities.Update(entity);
                await _mobiContext.SaveChangesAsync();
                return new ServiceResult(ResponseStatus.Success, null);
            }
            catch (Exception)
            {

                return new ServiceResult(ResponseStatus.ServerError, null);
            }
        }
        public async Task<ServiceResult> UpdateRangeAsync(IEnumerable<TEntity> entities)
        {
            try
            {
                Entities.UpdateRange(entities);
                await _mobiContext.SaveChangesAsync();
                return new ServiceResult(ResponseStatus.Success, null);
            }
            catch (Exception)
            {

                return new ServiceResult(ResponseStatus.ServerError, null);
            }
        }
        #endregion

        #region GetById
        public virtual async Task<TEntity> GetByIdAsync(params object[] ids)
        {
            return await Entities.FindAsync(ids);
        }
        public virtual TEntity GetById(params object[] ids)
        {
            return Entities.Find(ids);
        }
        #endregion

        #region Attach & Detach
        public async Task<TEntity> InsertReturnInformation(TEntity entity)
        {
            try
            {
                await Entities.AddAsync(entity);
                await _mobiContext.SaveChangesAsync();
                return entity;
            }
            catch (Exception)
            {
                return null!;
            }
        }
        #endregion
    }
}